using System;
using System.Linq;
using UnityEngine;

namespace GuildMasterIsekai.TileGrid
{
    /// <summary>Validates the current live grid state.</summary>
    internal static class GridEditing
    {
        public static bool CheckArea(GridData data, RectInt area, out string error)
        {
            error = null;

            if (area.width <= 0 || area.height <= 0 ||
                !data.Contains(area.min) || !data.Contains(area.max - Vector2Int.one))
            {
                error = "The complete brush must be inside the grid.";
                return false;
            }

            return true;
        }

        public static bool Validate(GridData data, out string error)
        {
            error = null;

            foreach (var p in data.Indexes.allPositionsWithin)
            {
                var cell = data[p.x, p.y];

                if (cell.Floor && (cell.BaseTile is not DbAutoTile floor || floor.Type != AutoTileType.Floor))
                {
                    error = "A built floor anchor must display a Floor tile.";
                    return false;
                }

                if (cell.Furniture && (!cell.Furniture.Tile || cell.BaseTile is not DbAutoTile tile ||
                                       tile.Type != cell.Furniture.BuildOn || cell.Toppings.Count !=
                                       (cell.Furniture.ToppingSlots?.Count ?? 0)))
                {
                    error = "Furniture requires an unoccupied cell with a matching BuildOn base type.";
                    return false;
                }

                if (!cell.Furniture && cell.Toppings.Count != 0 || cell.Toppings.Any(t => t && !t.Prefab))
                {
                    error = "Toppings require furniture slots and valid prefabs.";
                    return false;
                }
            }

            return true;
        }

    }
}