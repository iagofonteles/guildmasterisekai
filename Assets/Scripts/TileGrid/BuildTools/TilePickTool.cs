using System;
using System.Collections.Generic;
using Drafts.Inventory;
using TileGrid.Build;
using TileGrid.Tiles;
using UnityEngine;
using UnityEngine.Scripting;

namespace TileGrid.BuildTools
{
    [Serializable, Preserve]
    public sealed class TilePickTool : IBuildTool
    {
        public BuildLayerMask LayerMask => BuildLayerMask.All;

        public IEnumerable<ISlot> GetItems(GestureArgs e) => null;

        public IBuildCommand CreateCommand(GestureArgs e)
        {
            var pos = e.area.max - Vector2Int.one;
            var cell = e.grid[pos.x, pos.y];
            var curr = cell.Tiles[e.layer.Value];
            if (curr == null) return null;

            e.item.Value = curr;
            return null;
        }
    }
}