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
    [Serializable, Preserve]
    public sealed class ColorTool : IBuildTool
    {
        public BuildLayerMask LayerMask => BuildLayerMask.All;
        public IEnumerable<ISlot> GetItems(GestureArgs e) => e.bag.GetSlots<Color>();

        public IBuildCommand CreateCommand(GestureArgs e)
        {
            if (e.item.Value is Color color)
                e.color.Value = color;

            var cmd = new CellCommand(e.builder);

            foreach (var p in e.area.allPositionsWithin)
            {
                var cell = e.grid[p.x, p.y];
                var curr = cell.Tiles[e.layer.Value];
                if (curr == null) continue;

                var after = cell.Clone();
                after.Colors[e.layer.Value] = e.color.Value;
                after.Colors[BuildLayer2.Roof] = e.color.Value;
            }

            if (cmd.after.Count == 0)
                return e.Error("no change");
            return cmd;
        }
    }
}