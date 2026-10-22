using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TileGrid.Tiles;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

namespace TileGrid.Editor
{
    /// <summary>Bakes the two palette test tiles without changing their source PNGs.</summary>
    public static class IndexedPaletteConverter
    {
        public const string Directory = "Assets/Database/Tile/TestTile";
        public const string MaterialPath = Directory + "/Indexed Palette.mat";
        private const string PalettePath = "Assets/Sprites/Palette/palette jehkoba64-1x.png";
        private const int Shades = 8;
        private const int RampCount = 256;

        private sealed class Conversion
        {
            public string name;
            public Sprite source;
            public DbTile tile;
            public int width;
            public int height;
            public byte[] data;
            public int changedPixels;
            public readonly HashSet<int> changedColors = new();
            public readonly List<byte>[] regions = Enumerable.Range(0, 4)
                .Select(_ => new List<byte>()).ToArray();
        }

        [MenuItem("Game/Tiles/Convert Indexed Palette Test Tiles")]
        public static void ConvertTestTiles()
        {
            var palette = ReadPalette();
            // Validate both inputs completely before writing any generated assets.
            var conversions = new[] { Prepare("test floor", palette), Prepare("test tile", palette) };
            var shader = Shader.Find("Custom/TilemapIndexedPaletteProposal");
            if (!shader || ShaderUtil.ShaderHasError(shader))
                throw new InvalidOperationException("Indexed palette shader is missing or has compilation errors.");
            if (!SystemInfo.SupportsTextureFormat(TextureFormat.RG16))
                throw new NotSupportedException("This Editor device does not support RG16 textures.");

            var paletteTexture = WriteTexture(Directory + "/Indexed Palette.asset", 64, 1,
                TextureFormat.RGBA32, false, palette.SelectMany(c => new[] { c.r, c.g, c.b, (byte)255 }).ToArray());
            var ramps = new byte[Shades * RampCount];
            var nextRamp = 1;
            foreach (var conversion in conversions)
            foreach (var region in conversion.regions)
            {
                // The first eight rows document each input's original region ramps.
                // Ramp 0 bypasses the table and restores original colors on ANY tile.
                for (var slot = 0; slot < Shades; slot++)
                    ramps[nextRamp * Shades + slot] = region.Count == 0
                        ? (byte)0 : region[Math.Min(slot, region.Count - 1)];
                nextRamp++;
            }
            // Additional selectable ramps: eight consecutive palette entries each.
            // These are examples; artists can replace them with curated shade ramps.
            for (var row = nextRamp; row < RampCount; row++)
            for (var slot = 0; slot < Shades; slot++)
                ramps[row * Shades + slot] = (byte)(((row - nextRamp) * Shades + slot) % 64);
            var rampTexture = WriteTexture(Directory + "/Indexed Ramps.asset", Shades, RampCount,
                TextureFormat.R8, true, ramps);

            var material = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
            if (!material)
            {
                material = new Material(shader);
                AssetDatabase.CreateAsset(material, MaterialPath);
            }
            Undo.RecordObject(material, "Configure indexed palette material");
            material.shader = shader;
            material.SetTexture("_PaletteTex", paletteTexture);
            material.SetTexture("_RampTex", rampTexture);
            EditorUtility.SetDirty(material);

            foreach (var conversion in conversions)
            {
                var path = Directory + "/" + conversion.name + " indexed.asset";
                var texture = WriteTexture(path, conversion.width, conversion.height,
                    TextureFormat.RG16, true, conversion.data);
                var source = conversion.source;
                var sprite = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().SingleOrDefault();
                if (!sprite)
                {
                    sprite = Sprite.Create(texture, source.rect,
                        source.pivot / source.rect.size, source.pixelsPerUnit, 0,
                        SpriteMeshType.FullRect, source.border, false);
                    sprite.name = conversion.name + " indexed";
                    AssetDatabase.AddObjectToAsset(sprite, texture);
                }
                Undo.RecordObject(conversion.tile, "Use indexed test sprite");
                var serialized = new SerializedObject(conversion.tile);
                var rotations = serialized.FindProperty("rotations");
                rotations.arraySize = 1;
                rotations.GetArrayElementAtIndex(0).objectReferenceValue = sprite;
                // Keep normal RGB icons in UI; the packed data is not a color image.
                serialized.FindProperty("icon").objectReferenceValue = source;
                serialized.ApplyModifiedProperties();
                conversion.tile.color = Color.clear;
                conversion.tile.flags &= ~TileFlags.LockColor;
                EditorUtility.SetDirty(conversion.tile);
                Debug.Log($"Converted {conversion.name}: {conversion.width}x{conversion.height}, " +
                          $"{conversion.data.Length} packed bytes; quantized {conversion.changedPixels} pixels " +
                          $"from {conversion.changedColors.Count} off-palette colors. " +
                          $"Region shade counts: {string.Join(", ", conversion.regions.Select(r => r.Count))}.",
                    conversion.tile);
            }
            AssetDatabase.SaveAssets();
            Debug.Log($"Indexed palette material: {MaterialPath}. Assign it to a test TilemapRenderer. " +
                      "Set cell Color to Color.clear for original colors, or Color32(ramp1, ramp2, ramp3, ramp4). " +
                      "Existing hue presets are incompatible. Source PNGs and scene renderers were not changed.");
        }

        private static Conversion Prepare(string name, Color32[] palette)
        {
            var sourcePath = Directory + "/" + name + ".png";
            var tile = AssetDatabase.LoadAssetAtPath<DbTile>(Directory + "/" + name + ".asset");
            var sprites = AssetDatabase.LoadAllAssetsAtPath(sourcePath).OfType<Sprite>().ToArray();
            if (!tile || sprites.Length != 1)
                throw new InvalidDataException($"Expected one DbTile and one source sprite for {name}.");
            var source = ReadPng(sourcePath);
            var mask = ReadPng(Directory + "/" + name + " mask.png");
            try
            {
                if (source.width != mask.width || source.height != mask.height)
                    throw new InvalidDataException($"{name}: source and mask dimensions differ.");
                var conversion = new Conversion
                {
                    name = name, tile = tile, source = sprites[0],
                    width = source.width, height = source.height,
                    data = new byte[source.width * source.height * 2]
                };
                var pixels = source.GetPixels32();
                var masks = mask.GetPixels32();
                var indices = new byte[pixels.Length];
                var regions = new int[pixels.Length];
                for (var i = 0; i < pixels.Length; i++)
                {
                    var pixel = pixels[i];
                    if (pixel.a == 0) continue;
                    if (pixel.a != 255)
                        throw new InvalidDataException($"{name}: pixel {i} has partial alpha; packed format requires binary alpha.");
                    var region = Region(masks[i]);
                    var index = NearestIndex(pixel, palette);
                    indices[i] = index;
                    regions[i] = region;
                    if (!SameRgb(pixel, palette[index]))
                    {
                        conversion.changedPixels++;
                        conversion.changedColors.Add(pixel.r << 16 | pixel.g << 8 | pixel.b);
                    }
                    if (region > 0 && !conversion.regions[region - 1].Contains(index))
                        conversion.regions[region - 1].Add(index);
                }
                foreach (var region in conversion.regions)
                {
                    if (region.Count > Shades)
                        throw new InvalidDataException($"{name}: region has {region.Count} shades, maximum is {Shades}.");
                    region.Sort((a, b) =>
                    {
                        var order = Luminance(palette[a]).CompareTo(Luminance(palette[b]));
                        return order != 0 ? order : a.CompareTo(b);
                    });
                }
                for (var i = 0; i < pixels.Length; i++)
                {
                    if (pixels[i].a == 0) continue;
                    var region = regions[i];
                    var slot = region == 0 ? 0 : conversion.regions[region - 1].IndexOf(indices[i]);
                    conversion.data[i * 2] = (byte)(region * Shades + slot);
                    conversion.data[i * 2 + 1] = (byte)(indices[i] | 64);
                }
                return conversion;
            }
            finally
            {
                Object.DestroyImmediate(source);
                Object.DestroyImmediate(mask);
            }
        }

        private static Color32[] ReadPalette()
        {
            var texture = ReadPng(PalettePath);
            try
            {
                var colors = texture.GetPixels32();
                if (colors.Length != 64 || colors.Any(c => c.a != 255))
                    throw new InvalidDataException("Expected exactly 64 opaque palette pixels.");
                return colors;
            }
            finally { Object.DestroyImmediate(texture); }
        }

        private static Texture2D ReadPng(string path)
        {
            var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false, true);
            try
            {
                if (!ImageConversion.LoadImage(texture, File.ReadAllBytes(path), false))
                    throw new InvalidDataException($"Could not decode {path}.");
                return texture;
            }
            catch
            {
                Object.DestroyImmediate(texture);
                throw;
            }
        }

        private static Texture2D WriteTexture(string path, int width, int height,
            TextureFormat format, bool linear, byte[] bytes)
        {
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (!texture)
            {
                texture = new Texture2D(width, height, format, false, linear);
                texture.name = Path.GetFileNameWithoutExtension(path);
                AssetDatabase.CreateAsset(texture, path);
            }
            else if (texture.width != width || texture.height != height || texture.format != format ||
                     texture.isDataSRGB == linear)
                throw new InvalidDataException($"{path}: existing generated texture has incompatible settings.");
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.anisoLevel = 0;
            texture.LoadRawTextureData(bytes);
            texture.Apply(false, false);
            EditorUtility.SetDirty(texture);
            return texture;
        }

        private static int Region(Color32 mask)
        {
            if (mask.a == 0) return 0;
            if (mask.a != 255) throw new InvalidDataException("Mask requires binary alpha.");
            if (mask.r == 255 && mask.g == 0 && mask.b == 0) return 1;
            if (mask.r == 0 && mask.g == 255 && mask.b == 0) return 2;
            if (mask.r == 0 && mask.g == 0 && mask.b == 255) return 3;
            if (mask.r == 255 && mask.g == 255 && mask.b == 255) return 4;
            if (mask.r == 0 && mask.g == 0 && mask.b == 0) return 0;
            throw new InvalidDataException($"Unknown mask color {mask}.");
        }

        private static byte NearestIndex(Color32 pixel, Color32[] palette)
        {
            var bestDistance = int.MaxValue;
            byte best = 0;
            for (byte i = 0; i < palette.Length; i++)
            {
                var dr = pixel.r - palette[i].r;
                var dg = pixel.g - palette[i].g;
                var db = pixel.b - palette[i].b;
                var distance = dr * dr + dg * dg + db * db;
                if (distance >= bestDistance) continue;
                bestDistance = distance;
                best = i;
            }
            return best;
        }

        private static bool SameRgb(Color32 a, Color32 b) => a.r == b.r && a.g == b.g && a.b == b.b;
        private static int Luminance(Color32 c) => 2126 * c.r + 7152 * c.g + 722 * c.b;
    }
}
