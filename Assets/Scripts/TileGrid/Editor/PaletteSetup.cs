using System;
using System.Linq;
using Drafts.Database;
using TileGrid.Tiles;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TileGrid.Editor
{
    public static class PaletteSetup
    {
        [MenuItem("Game/Tiles/Setup Walls Palette")]
        public static void SetupWallPalettes()
        {
            var palette = Selection.objects.OfType<GameObject>()
                .FirstOrDefault(go => go.GetComponent<Grid>())?
                .GetComponent<Grid>();

            var db = Selection.objects.OfType<IDatabase>().FirstOrDefault();
            if (!palette || db == null) throw new Exception("Select database and palette");

            var walls = db.Items.OfType<ITileEntry>().Where(t => t.Roof != null).ToArray();
            var roofs = walls.Select(w => w.Roof).ToArray();

            // set palette tiles, target aspect 1.5/1
            // order: roof[i] on top of wall[i], one spacing on x and y
            //RXRXR
            //WXWXW
            //XXXXX
            //RXRXR
            //WXWXW
            if (walls.Length == 0) return;

            var columns = GetColumns(walls.Length, true);
            var map = PrepareTilemap(palette);
            for (var i = 0; i < walls.Length; i++)
            {
                var position = new Vector3Int(i % columns * 2, -(i / columns) * 3, 0);
                map.SetTile(position, roofs[i].Tile);
                map.SetTile(position + Vector3Int.down, walls[i].Tile);
            }

            FinishTilemap(map);
        }

        [MenuItem("Game/Tiles/Setup Floors Palette")]
        public static void SetupFloorPalettes()
        {
            var palette = Selection.objects.OfType<GameObject>()
                .FirstOrDefault(go => go.GetComponent<Grid>())?
                .GetComponent<Grid>();

            var db = Selection.objects.OfType<IDatabase>().FirstOrDefault();
            if (!palette || db == null) throw new Exception("Select database and palette");

            var walls = db.Items.OfType<ITileEntry>().Where(t => t.Roof != null).ToArray();
            var roofs = walls.Select(w => w.Roof).ToArray();
            var floors = db.Items.OfType<ITileEntry>().Except(walls).Except(roofs).ToArray();

            // set palette tiles, target aspect 1.5/1
            // order: alternate, one spacing on x and y
            //FXFXF
            //XFXFX
            //FXFXF
            //XFXFX
            if (floors.Length == 0) return;

            var columns = GetColumns(floors.Length, false);
            var map = PrepareTilemap(palette);
            for (var i = 0; i < floors.Length; i++)
            {
                var row = i / columns;
                var position = new Vector3Int(i % columns * 2 + row % 2, -row, 0);
                map.SetTile(position, floors[i].Tile);
            }

            FinishTilemap(map);
        }

        private static int GetColumns(int count, bool wallPairs)
        {
            var bestColumns = 1;
            var bestScore = float.PositiveInfinity;
            for (var columns = 1; columns <= count; columns++)
            {
                var rows = Mathf.CeilToInt((float)count / columns);
                var width = columns * 2 - 1;
                if (!wallPairs && (rows > 2 || count - columns > 1)) width++;
                var height = wallPairs ? rows * 3 - 1 : rows;
                var score = Mathf.Abs(Mathf.Log((float)width / height / 1.5f));
                if (score >= bestScore) continue;
                bestScore = score;
                bestColumns = columns;
            }

            return bestColumns;
        }

        private static Tilemap PrepareTilemap(Grid palette)
        {
            var map = palette.GetComponentInChildren<Tilemap>(true);
            if (!map)
            {
                var layer = new GameObject("Tilemap", typeof(Tilemap), typeof(TilemapRenderer));
                Undo.RegisterCreatedObjectUndo(layer, "Setup Tile Palette");
                Undo.SetTransformParent(layer.transform, palette.transform, "Setup Tile Palette");
                layer.transform.localPosition = Vector3.zero;
                layer.transform.localRotation = Quaternion.identity;
                layer.transform.localScale = Vector3.one;
                map = layer.GetComponent<Tilemap>();
            }

            Undo.RegisterCompleteObjectUndo(map, "Setup Tile Palette");
            map.ClearAllTiles();
            return map;
        }

        private static void FinishTilemap(Tilemap map)
        {
            map.CompressBounds();
            EditorUtility.SetDirty(map);
            if (PrefabUtility.IsPartOfPrefabInstance(map))
                PrefabUtility.RecordPrefabInstancePropertyModifications(map);
        }
    }
}
