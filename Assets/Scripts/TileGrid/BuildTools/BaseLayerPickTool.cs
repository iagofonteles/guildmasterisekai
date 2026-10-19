using System;
using System.Collections.Generic;
using Drafts.Inventory;
using TileGrid.Tiles;
using UnityEngine;
using UnityEngine.Scripting;

namespace GuildMasterIsekai.TileGrid
{
    [Serializable, Preserve]
    public sealed class BaseLayerPickTool : IBuildTool
    {
        [SerializeField] private Sprite icon;
        public Sprite Icon => icon;
        
        public IEnumerable<ISlot> GetItems(IBuildBag bag) => bag.GetItems<BaseTileSo>();

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
            BaseTileSo item = cell.Floor;

            if (!item && cell.BaseTile is DbAutoTile visible)
            {
                if (visible.Type == AutoTileType.Floor && visible.Item is FloorTileSo floor)
                    item = floor;
                else if (visible.Type is AutoTileType.Wall or AutoTileType.Roof && visible.Item is WallTileSo wall)
                    item = wall;
            }

            if (!item)
            {
                error = "There is no floor or wall material to pick at this cell.";
                return null;
            }

            e.builder.SelectedItem = item;
            error = null;
            return null;
        }
    }
}