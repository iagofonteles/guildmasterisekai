using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TileGrid.Editor
{
    /// <summary>Expands one RPG Maker-style two-column, three-row compact block into 47 blob tiles.</summary>
    public static class Tileset47Generator
    {
        [MenuItem("Game/Tiles/Expand Tileset to 47")]
        public static void GenerateFromSelected()
        {
            var texture = Selection.objects.OfType<Texture2D>().FirstOrDefault();

            if (!texture)
            {
                Debug.LogWarning(
                    "Select a compact PNG texture in the Project window before expanding the tileset.");
                return;
            }

            var path = Generate(AssetDatabase.GetAssetPath(texture));
            var output = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            Selection.activeObject = output;
            EditorGUIUtility.PingObject(output);
        }

        public const int VariantCount = 47;
        public const int Columns = 8;
        public const int Rows = 6;
        // Unity AutoTile's 3 x 3 mask, including the always-present center:
        //  64 128 256
        //   8  16  32
        //   1   2   4
        private const int Center = 16;
        private static readonly int[] masks = Enumerable.Range(0, 512)
            .Where(mask => (mask & Center) != 0 && NormalizeMask(mask) == mask).ToArray();
        public static IReadOnlyList<int> Masks => Array.AsReadOnly(masks);

        /// <summary>Creates or replaces the sibling PNG, preserving existing texture and sprite references.</summary>
        public static string Generate(string sourceAssetPath)
        {
            if (string.IsNullOrWhiteSpace(sourceAssetPath))
                throw new ArgumentNullException(nameof(sourceAssetPath));
            sourceAssetPath = sourceAssetPath.Replace('\\', '/');
            var projectRoot = Directory.GetParent(Application.dataPath).FullName;
            var fullSource = Path.GetFullPath(Path.Combine(projectRoot, sourceAssetPath));
            if (!sourceAssetPath.StartsWith("Assets/", StringComparison.Ordinal) ||
                !fullSource.StartsWith(
                    Application.dataPath.Replace('/', Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar,
                    StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(Path.GetExtension(sourceAssetPath), ".png", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Select a PNG under Assets.");
            var importer = AssetImporter.GetAtPath(sourceAssetPath) as TextureImporter;
            if (importer == null) throw new ArgumentException("Source must be an imported PNG texture.");
            var source = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            Texture2D expanded = null;

            try
            {
                // Decode original PNG bytes so Read/Write, compression and platform scaling
                // on the source importer cannot change the input pixels or dimensions.
                if (!ImageConversion.LoadImage(source, File.ReadAllBytes(fullSource), false))
                    throw new InvalidDataException("Could not decode source PNG.");
                expanded = Expand(source);
                var directory = Path.GetDirectoryName(sourceAssetPath).Replace('\\', '/');
                var name = Path.GetFileNameWithoutExtension(sourceAssetPath);
                var destination = $"{directory}/{name} 47.png";
                // Replace only PNG bytes; keep the existing .meta file and asset GUID.
                File.WriteAllBytes(Path.Combine(projectRoot, destination), expanded.EncodeToPNG());
                AssetDatabase.ImportAsset(destination, ImportAssetOptions.ForceSynchronousImport);
                ConfigureOutput(destination, importer.spritePixelsPerUnit, expanded.width / Columns,
                    expanded.height / Rows);
                Debug.Log(
                    $"Generated {destination}: {VariantCount} variants, {Columns} x {Rows} cells, {expanded.width} x {expanded.height} pixels.",
                    AssetDatabase.LoadAssetAtPath<Texture2D>(destination));
                return destination;
            }
            finally
            {
                Object.DestroyImmediate(source);
                if (expanded) Object.DestroyImmediate(expanded);
            }
        }

        public static string GenerateSelected()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            return Generate(path);
        }

        /// <summary>Returns a new readable texture. Caller owns and must destroy it.</summary>
        public static Texture2D Expand(Texture2D source)
        {
            if (!source) throw new ArgumentNullException(nameof(source));
            if (source.width % 4 != 0 || source.height % 6 != 0)
                throw new ArgumentException(
                    "Source dimensions must divide exactly into a 4 x 6 grid of tile quarters.", nameof(source));
            var tileWidth = source.width / 2;
            var tileHeight = source.height / 3;
            var quarterWidth = tileWidth / 2;
            var quarterHeight = tileHeight / 2;
            if (!source.isReadable)
                throw new ArgumentException(
                    "Expand requires a readable texture; Generate decodes the PNG directly.");
            if (masks.Length != VariantCount)
                throw new InvalidOperationException("Expected 47 canonical neighbor masks.");
            var input = source.GetPixels32();
            var pixels = new Color32[Columns * tileWidth * Rows * tileHeight];

            for (var index = 0; index < masks.Length; index++)
            for (var quadrant = 0; quadrant < 4; quadrant++)
            {
                var quarter = SourceQuarter(masks[index], quadrant);
                var dx = index % Columns * tileWidth + quadrant % 2 * quarterWidth;
                var dy = (Rows - 1 - index / Columns) * tileHeight + (quadrant < 2 ? quarterHeight : 0);
                // Quarter coordinates use the image's top left; pixel arrays use bottom left.
                var sx = quarter.x * quarterWidth;
                var sy = source.height - (quarter.y + 1) * quarterHeight;
                for (var y = 0; y < quarterHeight; y++)
                    Array.Copy(input, (sy + y) * source.width + sx,
                        pixels, (dy + y) * Columns * tileWidth + dx, quarterWidth);
            }

            var output = new Texture2D(Columns * tileWidth, Rows * tileHeight, TextureFormat.RGBA32, false)
                { name = source.name + " 47", filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp };
            output.SetPixels32(pixels);
            output.Apply(false, false);
            return output;
        }

        public static int NormalizeMask(int mask)
        {
            mask = (mask & 511) | Center;
            if ((mask & (8 | 128)) != (8 | 128)) mask &= ~64;
            if ((mask & (32 | 128)) != (32 | 128)) mask &= ~256;
            if ((mask & (8 | 2)) != (8 | 2)) mask &= ~1;
            if ((mask & (32 | 2)) != (32 | 2)) mask &= ~4;
            return mask;
        }

        // RPG Maker floor-style quarter selection, expressed as geometric edge rules
        // rather than a table of 48 shapes. Coordinates address the 4 x 6 quarter grid.
        // The compact sheet's upper-left tile preview is not a unique neighbor shape.
        private static Vector2Int SourceQuarter(int mask, int quadrant)
        {
            var right = quadrant % 2 != 0;
            var bottom = quadrant >= 2;
            var horizontal = (mask & (right ? 32 : 8)) != 0;
            var vertical = (mask & (bottom ? 2 : 128)) != 0;
            var diagonal = (mask & (bottom ? (right ? 4 : 1) : (right ? 256 : 64))) != 0;
            if (horizontal && vertical && !diagonal)
                return new Vector2Int(right ? 3 : 2, bottom ? 1 : 0); // concave corner
            return new Vector2Int(horizontal ? (right ? 1 : 2) : (right ? 3 : 0),
                vertical ? (bottom ? 3 : 4) : (bottom ? 5 : 2));
        }

        private static void ConfigureOutput(string path, float pixelsPerUnit, int tileWidth, int tileHeight)
        {
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.spritePixelsPerUnit = pixelsPerUnit > 0 ? pixelsPerUnit : tileWidth;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();

            var factories = new SpriteDataProviderFactories();
            factories.Init();
            var provider = factories.GetSpriteEditorDataProviderFromObject(importer);
            if (provider == null)
                throw new NotSupportedException("Output importer has no sprite editor data provider.");
            provider.InitSpriteEditorDataProvider();
            var capability = provider.GetDataProvider<ISpriteFrameEditCapability>();
            if (capability == null)
                throw new NotSupportedException("Output importer has no sprite edit capabilities.");
            var supported = capability.GetEditCapability();
            foreach (var required in new[]
                     {
                         EEditCapability.CreateAndDeleteSprite, EEditCapability.EditSpriteRect,
                         EEditCapability.EditSpriteName, EEditCapability.EditPivot
                     })
                if (!supported.HasCapability(required))
                    throw new NotSupportedException(
                        $"Output importer does not support {required}; slicing aborted.");
            var names = provider.GetDataProvider<ISpriteNameFileIdDataProvider>();
            if (names == null)
                throw new NotSupportedException("Output importer cannot maintain sprite name/file IDs.");
            var existingRects = provider.GetSpriteRects();
            var rects = new SpriteRect[VariantCount];

            for (var i = 0; i < rects.Length; i++)
            {
                var spriteName = $"{Path.GetFileNameWithoutExtension(path)}_{i:D2}_mask_{masks[i]:D3}";
                var existing = existingRects.FirstOrDefault(rect => rect.name == spriteName);
                rects[i] = new SpriteRect
                {
                    name = spriteName,
                    rect = new Rect(i % Columns * tileWidth, (Rows - 1 - i / Columns) * tileHeight, tileWidth,
                        tileHeight),
                    alignment = SpriteAlignment.Center,
                    pivot = new Vector2(.5f, .5f),
                    spriteID = existing != null ? existing.spriteID : GUID.Generate()
                };
            }

            provider.SetSpriteRects(rects);
            names.SetNameFileIdPairs(rects.Select(rect => new SpriteNameFileIdPair(rect.name, rect.spriteID)));
            provider.Apply();
            importer.SaveAndReimport();
            var sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();
            if (sprites.Length != VariantCount ||
                sprites.Any(sprite => sprite.rect.size != new Vector2(tileWidth, tileHeight)))
                throw new InvalidOperationException(
                    $"Generated sheet did not import as 47 sprites of {tileWidth} x {tileHeight} pixels.");
        }

        /// <summary>Checks mask coverage and pixel orientation against representative known RPG Maker shapes.</summary>
        public static string Validate()
        {
            const int TileSize = 32;
            const int QuarterSize = 16;
            var canonical = new HashSet<int>();
            for (var mask = 0; mask < 512; mask++) canonical.Add(NormalizeMask(mask));
            if (canonical.Count != VariantCount || !canonical.SetEquals(masks))
                throw new InvalidOperationException(
                    "Neighbor masks do not cover exactly 47 unique normalized shapes.");
            var source = new Texture2D(64, 96, TextureFormat.RGBA32, false);
            Texture2D output = null;

            try
            {
                var input = new Color32[64 * 96];
                for (var y = 0; y < 96; y++)
                for (var x = 0; x < 64; x++)
                    input[y * 64 + x] = new Color32((byte)x, (byte)y, 123, 255);
                source.SetPixels32(input);
                source.Apply();
                output = Expand(source);
                var actual = output.GetPixels32();
                var examples = new Dictionary<int, Vector2Int[]>
                {
                    [16] = new[]
                        { new Vector2Int(0, 2), new Vector2Int(3, 2), new Vector2Int(0, 5), new Vector2Int(3, 5) },
                    [511] = new[]
                        { new Vector2Int(2, 4), new Vector2Int(1, 4), new Vector2Int(2, 3), new Vector2Int(1, 3) },
                    [255] = new[]
                        { new Vector2Int(2, 4), new Vector2Int(3, 0), new Vector2Int(2, 3), new Vector2Int(1, 3) },
                    [56] = new[]
                        { new Vector2Int(2, 2), new Vector2Int(1, 2), new Vector2Int(2, 5), new Vector2Int(1, 5) },
                    [186] = new[]
                        { new Vector2Int(2, 0), new Vector2Int(3, 0), new Vector2Int(2, 1), new Vector2Int(3, 1) }
                };
                var checkedPixels = 0;

                foreach (var pair in examples)
                {
                    var index = Array.IndexOf(masks, pair.Key);
                    if (index < 0) throw new InvalidOperationException("Expected canonical example is missing.");

                    for (var q = 0; q < 4; q++)
                    for (var y = 0; y < QuarterSize; y++)
                    for (var x = 0; x < QuarterSize; x++)
                    {
                        var sx = pair.Value[q].x * QuarterSize + x;
                        var sy = 96 - (pair.Value[q].y + 1) * QuarterSize + y;
                        var dx = index % Columns * TileSize + q % 2 * QuarterSize + x;
                        var dy = (Rows - 1 - index / Columns) * TileSize + (q < 2 ? QuarterSize : 0) + y;
                        if (!actual[dy * output.width + dx].Equals(input[sy * 64 + sx]))
                            throw new InvalidOperationException(
                                $"Incorrect quarter/pixel orientation for mask {pair.Key}.");
                        checkedPixels++;
                    }
                }

                for (var y = 0; y < TileSize; y++)
                for (var x = 7 * TileSize; x < 8 * TileSize; x++)
                    if (actual[y * output.width + x].a != 0)
                        throw new InvalidOperationException("Unused final atlas cell must be transparent.");
                return
                    $"47 canonical shapes cover all 256 neighbor configurations; {checkedPixels} reference pixels match; unused cell is transparent.";
            }
            finally
            {
                Object.DestroyImmediate(source);
                if (output) Object.DestroyImmediate(output);
            }
        }
    }
}