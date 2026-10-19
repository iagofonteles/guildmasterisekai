using System;
using System.Collections.Generic;
using Drafts.Inventory;
using UnityEngine;
using UnityEngine.Scripting;

namespace GuildMasterIsekai.TileGrid
{
    [Serializable, Preserve]
    public sealed class BaseLayerColorPickTool : IBuildTool
    {
        public IEnumerable<ISlot> GetItems(IBuildBag bag) => bag.GetItems<Color>();
        public IBuildCommand CreateCommand(OperationArgs e, out string error)
        {
            if (e.grid == null) throw new ArgumentNullException(nameof(e.grid));
            if (!e.builder) throw new ArgumentNullException(nameof(e.builder));
            var position = e.area.max - Vector2Int.one;
            if (e.area.width <= 0 || e.area.height <= 0 || !e.grid.Contains(position))
            {
                error = "The picked cell is outside the grid.";
                return null;
            }

            var cell = e.grid[position.x, position.y];
            if (cell.BaseTile == null)
            {
                error = "There is no base color to pick at this cell.";
                return null;
            }

            e.builder.SelectedItem = cell.BaseColor;
            error = null;
            return null;
        }
    }
}