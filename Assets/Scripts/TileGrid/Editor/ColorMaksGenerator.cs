using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TileGrid.Tiles;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TileGrid.Editor
{
    public static class ColorMaksGenerator
    {
        private static Dictionary<Color32, int> _cachedPaletteIndexMap;
        private static readonly FieldInfo Rotations = typeof(DbTile).GetField("rotations",
            BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly Color32[] RegionColors =
        {
            new(255, 0, 0, 255), new(0, 255, 0, 255),
            new(0, 0, 255, 255), new(255, 255, 255, 255)
        };

        [InitializeOnLoadMethod]
        private static void Initialize() => DbTile.__GenerateMask = GenerateMask;

        internal static string MaskName(int rotation) => $"Color Mask {rotation}";

        public static void GenerateMask(DbTile tile) => GenerateMask(tile, IndexedPaletteConverter.GetConfig());

        public static void GenerateMask(DbTile tile, ColorPaletteConfig config)
        {
            var path = AssetDatabase.GetAssetPath(tile);
            if (!tile || string.IsNullOrEmpty(path) || !AssetDatabase.IsMainAsset(tile))
                throw new InvalidOperationException("Generate Mask requires a saved DbTile asset.");
            config.ValidateConfiguration();
            var sources = (Sprite[])Rotations.GetValue(tile);
            if (sources == null || sources.Length == 0 || sources.Any(s => !s))
                throw new InvalidDataException($"{tile.name}: assign every source rotation before generating masks.");

            var palette = IndexedPaletteConverter.ReadPalette(config);
            // Rebuild for each operation so palette edits cannot leave stale cached indices.
            _cachedPaletteIndexMap = new Dictionary<Color32, int>();
            for (var i = 0; i < palette.Length; i++)
                _cachedPaletteIndexMap.TryAdd(palette[i], i);

            var rotations = new int[sources.Length][];
            var ramps = new SortedSet<int>();
            var quantized = 0;
            // Validate every rotation and collect ramps before writing any subassets.
            for (var rotation = 0; rotation < sources.Length; rotation++)
            {
                var sprite = sources[rotation];
                var source = IndexedPaletteConverter.ReadPng(AssetDatabase.GetAssetPath(sprite));
                try
                {
                    var rect = sprite.rect;
                    if (source.width != sprite.texture.width || source.height != sprite.texture.height ||
                        rect != new Rect((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height))
                        throw new InvalidDataException($"{sprite.name}: source was resized/atlased or its rectangle uses fractional pixels.");
                    var width = (int)rect.width;
                    var height = (int)rect.height;
                    var pixels = source.GetPixels32();
                    var indices = new int[width * height];
                    rotations[rotation] = indices;
                    for (var y = 0; y < height; y++)
                    for (var x = 0; x < width; x++)
                    {
                        var pixel = pixels[((int)rect.y + y) * source.width + (int)rect.x + x];
                        var output = y * width + x;
                        indices[output] = -1;
                        if (pixel.a == 0) continue;
                        if (pixel.a != 255)
                            throw new InvalidDataException($"{sprite.name}: mask generation requires binary source alpha.");
                        if (!_cachedPaletteIndexMap.TryGetValue(pixel, out var index))
                        {
                            index = IndexedPaletteConverter.NearestIndex(pixel, palette);
                            _cachedPaletteIndexMap.Add(pixel, index);
                        }
                        if (!pixel.Equals(palette[index])) quantized++;
                        var ramp = index / config.PaletteColorShades;
                        ramps.Add(ramp);
                        indices[output] = ramp;
                    }
                }
                finally { Object.DestroyImmediate(source); }
            }
            if (ramps.Count > ColorPaletteConfig.RegionCount)
                throw new InvalidDataException($"{tile.name}: found {ramps.Count} palette ramps across rotations; " +
                                               $"the current shader supports only {ColorPaletteConfig.RegionCount} mask regions.");

            // Ascending palette order gives each ramp the same region in every rotation.
            var regions = ramps.Select((ramp, region) => (ramp, region))
                .ToDictionary(pair => pair.ramp, pair => pair.region);
            var assets = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Texture2D>().ToArray();
            var dirty = false;
            for (var rotation = 0; rotation < sources.Length; rotation++)
            {
                var width = (int)sources[rotation].rect.width;
                var height = (int)sources[rotation].rect.height;
                var pixels = rotations[rotation]
                    .Select(ramp => ramp < 0 ? new Color32(0, 0, 0, 0) : RegionColors[regions[ramp]]).ToArray();
                var name = MaskName(rotation);
                var mask = assets.FirstOrDefault(t => t.name == name);
                var changed = false;
                if (!mask)
                {
                    mask = new Texture2D(width, height, TextureFormat.RGBA32, false, true) { name = name };
                    AssetDatabase.AddObjectToAsset(mask, tile);
                    changed = true;
                }
                else if (mask.width != width || mask.height != height || mask.format != TextureFormat.RGBA32 ||
                         mask.isDataSRGB || mask.mipmapCount != 1)
                {
                    if (!mask.Reinitialize(width, height,
                            UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm, false))
                        throw new InvalidOperationException($"Could not resize {name}.");
                    changed = true;
                }
                if (changed || mask.filterMode != FilterMode.Point || mask.wrapMode != TextureWrapMode.Clamp ||
                    mask.anisoLevel != 0 || !mask.GetPixels32().SequenceEqual(pixels))
                {
                    mask.filterMode = FilterMode.Point;
                    mask.wrapMode = TextureWrapMode.Clamp;
                    mask.anisoLevel = 0;
                    mask.SetPixels32(pixels);
                    mask.Apply(false, false);
                    EditorUtility.SetDirty(mask);
                    dirty = true;
                }
            }
            if (dirty)
            {
                EditorUtility.SetDirty(tile);
                AssetDatabase.SaveAssetIfDirty(tile);
            }
            Debug.Log($"Generated {sources.Length} color masks for {tile.name}. Palette ramps " +
                      $"{string.Join(", ", ramps.Select(ramp => ramp + 1))} map to regions 1..{ramps.Count}; " +
                      $"quantized {quantized} off-palette pixels.", tile);
        }
    }
}
