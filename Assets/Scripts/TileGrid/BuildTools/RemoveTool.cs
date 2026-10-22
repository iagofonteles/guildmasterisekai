using TileGrid.Build;
using TileGrid.Data;
using TileGrid.Tiles;
using UnityEngine;

namespace TileGrid.BuildTools
{
    public sealed class RemoveTool
    {
        public IBuildCommand CreateCommand(BuildArgs e)
        {
            var cmd = new CellCommand(e.builder);

            foreach (var p in e.area.allPositionsWithin)
            {
                var cell = e.grid[p.x, p.y];
                if (cell.Tiles[e.layer.Value] == null) continue;

                var after = cell.Clone();
                after.Tiles[e.layer.Value] = null;
                after.Colors[e.layer.Value] = null;
                after.Tiles[BuildLayer.Roof] = null;
                after.Colors[BuildLayer.Roof] = null;

                cmd.after.Add(after);
                cmd.before.Add(cell.Clone());
            }

            if (cmd.after.Count == 0)
                return e.Error("no change");

            cmd.cost.AddItem(e.builder.Material, -cmd.after.Count);
            return cmd;
        }
    }
}