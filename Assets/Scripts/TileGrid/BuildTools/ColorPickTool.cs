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
    public sealed class ColorPickTool : IBuildTool
    {
        public BuildLayerMask LayerMask => BuildLayerMask.All;

        public IEnumerable<ISlot> GetItems(BuildArgs e) => null;

        public IBuildCommand CreateCommand(BuildArgs e)
        {
            var pos = e.area.max - Vector2Int.one;
            var cell = e.grid[pos.x, pos.y];
            var curr = cell.Tiles[e.layer.Value];
            if (curr == null) return null;

            e.color.Value = cell.Colors[e.layer.Value];
            return null;
        }
    }
}