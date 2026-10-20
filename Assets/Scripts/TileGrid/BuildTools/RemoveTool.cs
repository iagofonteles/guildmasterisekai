using System;
using System.Collections.Generic;
using Drafts.Inventory;
using TileGrid.Build;
using TileGrid.Data;
using TileGrid.Tiles;
using UnityEngine;
using UnityEngine.Scripting;

namespace TileGrid.BuildTools
{
    [Preserve, Serializable]
    public sealed class RemoveTool : IBuildTool
    {
        public BuildLayerMask LayerMask => BuildLayerMask.All;

        public IEnumerable<ISlot> GetItems(GestureArgs e) => null;

        public IBuildCommand CreateCommand(GestureArgs e)
        {
            var cmd = new CellCommand(e.builder);

            foreach (var p in e.area.allPositionsWithin)
            {
                var cell = e.grid[p.x, p.y];
                if (cell.Tiles[e.layer.Value] == null) continue;

                var after = cell.Clone();
                after.Tiles[e.layer.Value] = null;
                after.Colors[e.layer.Value] = Color.clear;
                after.Tiles[BuildLayer2.Roof] = null;
                after.Colors[BuildLayer2.Roof] = Color.clear;

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