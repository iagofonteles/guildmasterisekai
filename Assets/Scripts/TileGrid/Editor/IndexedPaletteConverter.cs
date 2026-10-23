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
    /// <summary>Bakes indexed sprites into their owning tile without changing source images.</summary>
    public static class IndexedPaletteConverter
    {
        private const BindingFlags Fields = BindingFlags.Instance | BindingFlags.NonPublic;
        private static readonly FieldInfo DbTileRotations = typeof(DbTile).GetField("rotations", Fields);
        private static readonly FieldInfo DbTileEncodedRotations = typeof(DbTile).GetField("encodedRotations", Fields);
        private static readonly FieldInfo DbAutoTileRotations = typeof(DbAutoTile).GetField("", Fields);
        private static readonly FieldInfo DbAutoTileEncodedRotations = typeof(DbAutoTile).GetField("", Fields);

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            DbTile.__EncodeSprites = t => EncodeSprites(t, DbTileRotations, DbTileEncodedRotations);
            // DbAutoTile.__EncodeSprites = t => EncodeSprites(t, DbAutoTileRotations, DbAutoTileEncodedRotations);
        }

        private sealed class Conversion
        {
            public string name;
            public Sprite source;
            public int width;
            public int height;
            public byte[] data;
            public int changedPixels;
            public readonly HashSet<int> changedColors = new();
        }

        public static void EncodeSprites(Object tile, FieldInfo originalField, FieldInfo encodedField,
            ColorPaletteConfig config = null)
        {
            var path = AssetDatabase.GetAssetPath(tile);
            if (!tile || string.IsNullOrEmpty(path) || !AssetDatabase.IsMainAsset(tile))
                throw new InvalidOperationException("Encode Sprites requires a saved DbTile asset.");
            if (!SystemInfo.SupportsTextureFormat(TextureFormat.RG16))
                throw new NotSupportedException("This Editor device does not support RG16 textures.");
            var sources = (Sprite[])originalField.GetValue(tile);
            if (sources == null || sources.Length == 0 || sources.Any(s => !s))
                throw new InvalidDataException($"{tile.name}: assign every source rotation before encoding.");
            if (!config) config = GetConfig();
            config.ValidateConfiguration();
            var palette = ReadPalette(config);
            var assets = AssetDatabase.LoadAllAssetsAtPath(path);
            // Validate all rotations before modifying persistent assets.
            var conversions = sources.Select(source => Prepare(source, palette, config.PaletteColorShades)).ToArray();
            var previous = (Sprite[])encodedField.GetValue(tile);
            var encoded = new Sprite[sources.Length];

            for (var i = 0; i < sources.Length; i++)
            {
                var conversion = conversions[i];
                var source = sources[i];
                var spriteName = $"Indexed Sprite {i}";
                var textureName = $"Indexed Texture {i}";
                var sprite = previous != null && i < previous.Length && previous[i] &&
                             AssetDatabase.GetAssetPath(previous[i]) == path
                    ? previous[i]
                    : assets.OfType<Sprite>().FirstOrDefault(s => s.name == spriteName);
                var texture = sprite && AssetDatabase.GetAssetPath(sprite.texture) == path
                    ? sprite.texture
                    : assets.OfType<Texture2D>().FirstOrDefault(t => t.name == textureName);

                if (!texture)
                {
                    texture = new Texture2D(conversion.width, conversion.height, TextureFormat.RG16, false, true);
                    texture.name = textureName;
                    AssetDatabase.AddObjectToAsset(texture, tile);
                }

                if (texture.width != conversion.width || texture.height != conversion.height ||
                    texture.format != TextureFormat.RG16 || texture.isDataSRGB || texture.mipmapCount != 1)
                {
                    if (!texture.Reinitialize(conversion.width, conversion.height,
                            UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8_UNorm, false))
                        throw new InvalidOperationException($"Could not resize {textureName}.");
                    EditorUtility.SetDirty(texture);
                }

                if (!texture.GetRawTextureData<byte>().ToArray().SequenceEqual(conversion.data) ||
                    texture.filterMode != FilterMode.Point || texture.wrapMode != TextureWrapMode.Clamp ||
                    texture.anisoLevel != 0)
                {
                    texture.filterMode = FilterMode.Point;
                    texture.wrapMode = TextureWrapMode.Clamp;
                    texture.anisoLevel = 0;
                    texture.LoadRawTextureData(conversion.data);
                    texture.Apply(false, false);
                    EditorUtility.SetDirty(texture);
                }

                var rect = new Rect(0, 0, conversion.width, conversion.height);
                var pivot = source.pivot / source.rect.size;

                if (!sprite || sprite.texture != texture || sprite.rect != rect ||
                    sprite.pivot / sprite.rect.size != pivot || sprite.pixelsPerUnit != source.pixelsPerUnit ||
                    sprite.border != source.border)
                {
                    var replacement = Sprite.Create(texture, rect, pivot, source.pixelsPerUnit,
                        0, SpriteMeshType.FullRect, source.border, false);
                    replacement.name = spriteName;

                    if (sprite)
                    {
                        EditorUtility.CopySerialized(replacement, sprite);
                        Object.DestroyImmediate(replacement);
                        EditorUtility.SetDirty(sprite);
                    }
                    else
                    {
                        sprite = replacement;
                        AssetDatabase.AddObjectToAsset(sprite, tile);
                    }
                }

                encoded[i] = sprite;
                Debug.Log($"Encoded {tile.name} rotation {i}: {conversion.width}x{conversion.height}; " +
                          $"quantized {conversion.changedPixels} pixels from " +
                          $"{conversion.changedColors.Count} off-palette colors.", tile);
            }

            if (previous == null || !previous.SequenceEqual(encoded))
            {
                encodedField.SetValue(tile, encoded);
                EditorUtility.SetDirty(tile);
            }

            AssetDatabase.SaveAssetIfDirty(tile);
        }

        private static Conversion Prepare(Sprite sprite, Color32[] palette, int shades)
        {
            var secondary = new SecondarySpriteTexture[sprite.GetSecondaryTextureCount()];
            sprite.GetSecondaryTextures(secondary);
            var maskTexture = secondary.FirstOrDefault(t => t.name == "_MaskTex").texture;
            if (!maskTexture)
                throw new InvalidDataException($"{sprite.name}: missing _MaskTex secondary texture.");
            var source = ReadPng(AssetDatabase.GetAssetPath(sprite));
            Texture2D mask = null;

            try
            {
                mask = ReadTexture(maskTexture);
                var maskRect = new Rect(0, 0, source.width, source.height);
                if (mask.width != maskRect.width || mask.height != maskRect.height ||
                    source.width != sprite.texture.width || source.height != sprite.texture.height)
                    throw new InvalidDataException(
                        $"{sprite.name}: source/mask dimensions differ or source was resized/atlased.");
                var rect = sprite.rect;
                var x = (int)rect.x;
                var y = (int)rect.y;
                var width = (int)rect.width;
                var height = (int)rect.height;
                if (rect != new Rect(x, y, width, height))
                    throw new InvalidDataException($"{sprite.name}: sprite rectangle must use whole pixels.");
                var conversion = new Conversion
                {
                    name = sprite.name, source = sprite, width = width, height = height,
                    data = new byte[width * height * 2]
                };
                var pixels = source.GetPixels32();
                var masks = mask.GetPixels32();

                for (var row = 0; row < height; row++)
                for (var column = 0; column < width; column++)
                {
                    var sourceIndex = (y + row) * source.width + x + column;
                    var pixel = pixels[sourceIndex];
                    if (pixel.a == 0) continue;
                    if (pixel.a != 255)
                        throw new InvalidDataException($"{sprite.name}: pixel {sourceIndex} requires binary alpha.");
                    var region = Region(masks[sourceIndex]);
                    var index = NearestIndex(pixel, palette);

                    if (!SameRgb(pixel, palette[index]))
                    {
                        conversion.changedPixels++;
                        conversion.changedColors.Add(pixel.r << 16 | pixel.g << 8 | pixel.b);
                    }

                    var output = (row * width + column) * 2;
                    // Preserve the source shade column within the configured natural ramps.
                    conversion.data[output] = (byte)(region * ColorPaletteConfig.EncodedShades +
                                                     (region == 0 ? 0 : index % shades));
                    // G bit 6 stores opacity; bit 7 stores the palette index's high bit.
                    conversion.data[output + 1] = (byte)((index & 63) | ((index & 64) << 1) |
                                                        ColorPaletteConfig.OpaqueFlag);
                }

                return conversion;
            }
            finally
            {
                Object.DestroyImmediate(source);
                if (mask) Object.DestroyImmediate(mask);
            }
        }

        internal static ColorPaletteConfig GetConfig()
        {
            var configs = AssetDatabase.FindAssets("t:ColorPaletteConfig");
            if (configs.Length != 1)
                throw new InvalidOperationException("Assign a ColorPaletteConfig explicitly, or keep exactly one config asset.");
            return AssetDatabase.LoadAssetAtPath<ColorPaletteConfig>(AssetDatabase.GUIDToAssetPath(configs[0]));
        }

        internal static Color32[] ReadPalette(ColorPaletteConfig config)
        {
            var source = config.PaletteTexture;
            var path = AssetDatabase.GetAssetPath(source);
            var isPng = string.Equals(Path.GetExtension(path), ".png", StringComparison.OrdinalIgnoreCase);
            var texture = isPng ? ReadPng(path) : source;

            try
            {
                if (!texture.isReadable)
                    throw new InvalidDataException($"{source.name}: palette texture must be readable for encoding.");
                var colors = texture.GetPixels32().Take(config.PaletteColorCount).ToArray();
                if (colors.Length != config.PaletteColorCount || colors.Any(c => c.a != 255))
                    throw new InvalidDataException("Expected the configured number of opaque palette pixels.");
                return colors;
            }
            finally
            {
                if (isPng) Object.DestroyImmediate(texture);
            }
        }

        internal static Texture2D ReadTexture(Texture2D texture)
        {
            var path = AssetDatabase.GetAssetPath(texture);
            if (string.Equals(Path.GetExtension(path), ".png", StringComparison.OrdinalIgnoreCase))
                return ReadPng(path);
            if (!texture.isReadable)
                throw new InvalidDataException($"{texture.name}: generated texture must be readable.");
            return Object.Instantiate(texture);
        }

        internal static Texture2D ReadPng(string path)
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

        internal static byte NearestIndex(Color32 pixel, Color32[] palette)
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
    }
}
