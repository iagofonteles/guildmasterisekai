using System;
using System.Collections.Generic;
using Drafts.Inventory;
using UnityEngine;
using UnityEngine.Scripting;

namespace GuildMasterIsekai.TileGrid
{
    [Serializable, Preserve]
    public sealed class BaseLayerColorTool : IBuildTool
    {
        public IEnumerable<ISlot> GetItems(IBuildBag bag) => bag.GetItems<Color>();

        public IBuildCommand CreateCommand(OperationArgs e, out string error)
        {
            e.Validate();

            if (e.item is not Color color)
            {
                error = "no color selected";
                return null;
            }

            var changes = new Dictionary<Vector2Int, GridCell>();

            void Paint(Vector2Int position, AutoTileType? type = null)
            {
                if (!e.grid.Contains(position)) return;
                var cell = e.grid[position.x, position.y];
                if (!cell.BaseTile?.Tile) return;
                if (type.HasValue && (cell.BaseTile is not DbAutoTile tile || tile.Type != type)) return;
                if (cell.BaseColor.Equals(color) || changes.ContainsKey(position)) return;
                var after = (GridCell)cell.Clone();
                after.BaseColor = color;
                changes.Add(position, after);
            }

            foreach (var p in e.area.allPositionsWithin)
            {
                if (!e.grid.Contains(p)) continue;
                Paint(p);
                if (e.grid[p.x, p.y].BaseTile is not DbAutoTile current) continue;
                if (current.Type == AutoTileType.Wall)
                    Paint(p + Vector2Int.up, AutoTileType.Roof);
                else if (current.Type == AutoTileType.Roof)
                    Paint(p + Vector2Int.down, AutoTileType.Wall);
            }

            if (changes.Count == 0)
            {
                error = "no changes.";
                return null;
            }

            var cmd = new CellCommand(e.builder);
            foreach (var pair in changes)
            {
                cmd.before.Add(e.grid[pair.Key.x, pair.Key.y].Clone());
                cmd.after.Add(pair.Value);
            }

            error = null;
            return cmd;
        }
    }
}
