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
            var directory = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(tiles[0]));

            for (var i = 0; i < sprites.Length; i++)
            {
                var tile = i < tiles.Length ? tiles[i] : Object.Instantiate(tiles[0]);
                var sprite = sprites[i];
                tile.name = sprite.name;
                ((Sprite[])field.GetValue(tile))[0] = sprite;

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

                EditorUtility.SetDirty(tile);
            }

            AssetDatabase.SaveAssets();
        }
    }
}