using System;
using System.Linq;
using System.Reflection;
using TileGrid.Tiles;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TileGrid.Editor
{
    public static class TileGenerator
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;

        [MenuItem("Game/Tiles/Create DbTiles from base")]
        public static void GenerateFromSelected()
        {
            var tiles = Selection.objects.OfType<DbTile>().ToArray();
            if (tiles.Length == 0) throw new Exception("no DbTile selected as base");

            var sprites = Selection.objects.OfType<Sprite>().ToArray();
            var field = typeof(DbTile).GetField("rotations", Flags)!;
            var rotationCount = ((Sprite[])field.GetValue(tiles[0])).Length;
            if (rotationCount < 1) throw new Exception("base tile must have at least one rotation slot");
            if (sprites.Length == 0 || sprites.Length % rotationCount != 0)
                throw new Exception($"select complete groups of {rotationCount} rotation sprites");
            Array.Sort(sprites, (a, b) =>
            {
                var pathOrder = string.CompareOrdinal(AssetDatabase.GetAssetPath(a), AssetDatabase.GetAssetPath(b));
                return pathOrder != 0 ? pathOrder : EditorUtility.NaturalCompare(a.name, b.name);
            });
            var directory = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(tiles[0]));

            for (var i = 0; i < sprites.Length / rotationCount; i++)
            {
                var tile = i < tiles.Length ? tiles[i] : Object.Instantiate(tiles[0]);
                var rotations = new Sprite[rotationCount];
                Array.Copy(sprites, i * rotationCount, rotations, 0, rotationCount);
                var sprite = rotations[0];
                tile.name = sprite.name;
                field.SetValue(tile, rotations);

                if (i < tiles.Length)
                {
                    var path = AssetDatabase.GetAssetPath(tile);
                    AssetDatabase.RenameAsset(path, sprite.name);
                }
                else
                {
                    tile.__RerollId();
                    var path = $"{directory}/{sprite.name}.asset";
                    path = AssetDatabase.GenerateUniqueAssetPath(path);
                    AssetDatabase.CreateAsset(tile, path);
                }

                DbTile.__EncodeSprites(tile); //TODO
                EditorUtility.SetDirty(tile);
            }

            AssetDatabase.SaveAssets();
        }
    }
}
