using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TileGrid.Editor
{
    public static class ColorMaksGenerator
    {
        private const string MenuPath = "Game/Tiles/GenerateColorMask";
        private const string MaskName = "_MaskTex";
        private static readonly Color32[] RegionColors =
        {
            new(255, 0, 0, 255), new(0, 255, 0, 255),
            new(0, 0, 255, 255), new(255, 255, 255, 255)
        };

        private static string[] SelectedPaths() => Selection.objects.OfType<Texture2D>()
            .Where(texture => AssetDatabase.IsMainAsset(texture)).Select(texture => AssetDatabase.GetAssetPath(texture))
            .Where(path => AssetImporter.GetAtPath(path) is TextureImporter
                { textureType: TextureImporterType.Sprite } &&
                string.Equals(Path.GetExtension(path), ".png", StringComparison.OrdinalIgnoreCase))
            .Distinct().ToArray();

        [MenuItem(MenuPath, true)]
        private static bool ValidateSelection() => SelectedPaths().Length > 0;

        [MenuItem(MenuPath)]
        public static void GenerateColorMask()
        {
            var paths = SelectedPaths();
            var config = IndexedPaletteConverter.GetConfig();
            config.ValidateConfiguration();
            var palette = IndexedPaletteConverter.ReadPalette(config);
            foreach (var path in paths)
            {
                try { GenerateMask(path, palette, config.PaletteColorShades); }
                catch (Exception exception) { Debug.LogException(exception); }
            }
        }

        private static void GenerateMask(string path, Color32[] palette, int shades)
        {
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            var source = IndexedPaletteConverter.ReadPng(path);
            try
            {
                var indices = new int[source.width * source.height];
                var counts = new Dictionary<int, long>();
                var paletteIndices = new Dictionary<Color32, int>();
                for (var i = 0; i < palette.Length; i++) paletteIndices.TryAdd(palette[i], i);
                var pixels = source.GetPixels32();
                var quantized = 0;
                for (var i = 0; i < pixels.Length; i++)
                {
                    var pixel = pixels[i];
                    indices[i] = -1;
                    if (pixel.a == 0) continue;
                    if (pixel.a != 255)
                        throw new InvalidDataException($"{path}: mask generation requires binary source alpha.");
                    if (!paletteIndices.TryGetValue(pixel, out var index))
                    {
                        index = IndexedPaletteConverter.NearestIndex(pixel, palette);
                        paletteIndices.Add(pixel, index);
                    }
                    if (!pixel.Equals(palette[index])) quantized++;
                    var ramp = index / shades;
                    indices[i] = ramp;
                    counts.TryGetValue(ramp, out var count);
                    counts[ramp] = count + 1;
                }

                // Rank the entire sheet, keeping assignments consistent across its sprites.
                var ranked = counts.OrderByDescending(pair => pair.Value).ThenBy(pair => pair.Key).ToArray();
                var regions = ranked.Take(ColorPaletteConfig.RegionCount)
                    .Select((pair, region) => (ramp: pair.Key, region))
                    .ToDictionary(pair => pair.ramp, pair => pair.region);
                var maskPixels = indices.Select(ramp => ramp < 0 ? new Color32(0, 0, 0, 0) :
                    regions.TryGetValue(ramp, out var region) ? RegionColors[region] :
                    new Color32(0, 0, 0, 255)).ToArray();

                // Imported image assets cannot persist manually added subassets.
                var assetPath = path + ".ColorMask.asset";
                var owner = AssetDatabase.LoadAssetAtPath<ColorMaskAsset>(assetPath);
                if (!owner)
                {
                    if (AssetDatabase.LoadMainAssetAtPath(assetPath))
                        throw new InvalidDataException($"{assetPath}: already contains another asset type.");
                    owner = ScriptableObject.CreateInstance<ColorMaskAsset>();
                    AssetDatabase.CreateAsset(owner, assetPath);
                }
                var mask = AssetDatabase.LoadAllAssetsAtPath(assetPath).OfType<Texture2D>()
                    .FirstOrDefault(texture => texture.name == MaskName);
                if (!mask)
                {
                    mask = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false, true)
                        { name = MaskName };
                    AssetDatabase.AddObjectToAsset(mask, owner);
                }
                else if (mask.width != source.width || mask.height != source.height ||
                         mask.format != TextureFormat.RGBA32 || mask.isDataSRGB || mask.mipmapCount != 1)
                {
                    if (!mask.Reinitialize(source.width, source.height,
                            UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm, false))
                        throw new InvalidOperationException($"Could not resize {assetPath}.");
                }
                mask.filterMode = FilterMode.Point;
                mask.wrapMode = TextureWrapMode.Clamp;
                mask.anisoLevel = 0;
                mask.SetPixels32(maskPixels);
                mask.Apply(false, false);
                EditorUtility.SetDirty(mask);
                EditorUtility.SetDirty(owner);
                AssetDatabase.SaveAssetIfDirty(owner);

                var secondary = (importer.secondarySpriteTextures ?? Array.Empty<SecondarySpriteTexture>())
                    .Where(texture => texture.name != MaskName).ToList();
                secondary.Add(new SecondarySpriteTexture { name = MaskName, texture = mask });
                importer.secondarySpriteTextures = secondary.ToArray();
                importer.SaveAndReimport();

                var report = ranked.Select(pair => $"{pair.Key + 1}: {pair.Value} pixels -> " +
                    (regions.TryGetValue(pair.Key, out var region) ? $"region {region + 1}" : "black (fixed)"));
                Debug.Log($"Generated color mask for {path}. Palette ramps {string.Join(", ", report)}; " +
                          $"quantized {quantized} off-palette pixels.",
                    AssetDatabase.LoadAssetAtPath<Texture2D>(path));
            }
            finally { Object.DestroyImmediate(source); }
        }
    }
}
