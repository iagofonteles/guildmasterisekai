using System;
using System.Collections.Generic;
using Drafts.Inventory;
using TileGrid.Build;
using TileGrid.Data;
using TileGrid.Tiles;
using UnityEngine;
using UnityEngine.Scripting;

namespace GuildMasterIsekai.TileGrid
{
    [Serializable, Preserve]
    public sealed class ColorTool : IBuildTool
    {
        public BuildLayerMask LayerMask => BuildLayerMask.All;
        public IEnumerable<ISlot> GetItems(GestureArgs e) => e.bag.GetSlots<Color>();

        public IBuildCommand CreateCommand(GestureArgs e)
        {
            if (e.item is Color color)
                e.color = color;

            var cmd = new CellCommand(e.builder);

            foreach (var p in e.area.allPositionsWithin)
            {
                var cell = e.grid[p.x, p.y];
                var curr = cell.Tiles[e.layer];
                if (curr == null) continue;

                var after = cell.Clone();
                after.Colors[e.layer] = e.color;
                after.Colors[BuildLayer2.Roof] = e.color;
            }

            if (cmd.after.Count == 0)
                return e.Error("no change");
            return cmd;
        }
    }
}