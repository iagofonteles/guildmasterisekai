using System;
using TileGrid.Build;
using TileGrid.Data;
using TileGrid.Tiles;
using UnityEngine.Scripting;

namespace TileGrid.BuildTools
{
    [Serializable, Preserve]
    public sealed class ReplaceTool : IBuildTool
    {
        public BuildLayerMask LayerMask => BuildLayerMask.All;

        public IBuildCommand CreateCommand(GestureArgs e)
        {
            if (e.item is not ITileEntry tile)
                return e.Error("not ITileEntry");
            if (((int)tile.LayerMask & e.layer) == 0)
                return e.Error("incompatible layer");

            var cmd = new CellCommand(e.builder);

            foreach (var p in e.area.allPositionsWithin)
            {
                var cell = e.grid[p.x, p.y];
                var curr = cell.Tiles[e.layer];
                if (curr == tile) continue;
                if (!tile.CanReplace(curr)) continue;

                var after = cell.Clone();
                after.Tiles[e.layer] = tile;
                after.Colors[e.layer] = e.color;
                after.Tiles[BuildLayer2.Roof] = tile.Roof;
                after.Colors[BuildLayer2.Roof] = e.color;

                cmd.after.Add(after);
                cmd.before.Add(cell.Clone());
            }

            if (cmd.after.Count == 0)
                return e.Error("no change");
            return cmd;
        }
    }
}