using System;
using TileGrid.Build;
using TileGrid.Data;
using TileGrid.Tiles;
using UnityEngine;
using UnityEngine.Scripting;

namespace TileGrid.BuildTools
{
    [Preserve, Serializable]
    public sealed class PlaceTool : IBuildTool
    {
        public BuildLayerMask LayerMask => BuildLayerMask.All;

        public IBuildCommand CreateCommand(GestureArgs e)
        {
            if (e.item.Value is not ITileEntry tile)
                return e.Error("not ITileEntry");
            if (((int)tile.LayerMask & (1 << e.layer.Value)) == 0)
                return e.Error("incompatible layer");

            if (tile is DbTile t && t.Size != Vector2Int.one)
                return PlaceSizedTile(e, t);

            var cmd = new CellCommand(e.builder);
            var cost = 0;

            foreach (var p in e.area.allPositionsWithin)
            {
                var cell = e.grid[p.x, p.y];
                var curr = cell.Tiles[e.layer.Value];
                if (curr == tile) continue;

                var after = cell.Clone();
                after.Tiles[e.layer.Value] = tile;
                after.Colors[e.layer.Value] = e.color.Value;
                after.Tiles[BuildLayer2.Roof] = tile.Roof;
                after.Colors[BuildLayer2.Roof] = e.color.Value;

                cmd.after.Add(after);
                cmd.before.Add(cell.Clone());

                if (curr == null) cost++;
            }

            if (cmd.after.Count == 0)
                return e.Error("no change");

            cmd.cost.AddItem(e.builder.Material, cost);
            return cmd;
        }

        private IBuildCommand PlaceSizedTile(GestureArgs gestureArgs, DbTile dbTile)
        {
            throw new NotImplementedException();
        }
    }
}