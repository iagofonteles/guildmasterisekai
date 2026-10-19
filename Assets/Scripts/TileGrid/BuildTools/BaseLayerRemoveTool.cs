using System;
using System.Collections.Generic;
using System.Linq;
using Drafts.Inventory;
using UnityEngine;
using UnityEngine.Scripting;

namespace GuildMasterIsekai.TileGrid
{
    [Serializable, Preserve]
    public sealed class BaseLayerRemoveTool : IBuildTool
    {
        public IEnumerable<ISlot> GetItems(IBuildBag bag) => throw new InvalidOperationException();

        public IBuildCommand CreateCommand(OperationArgs e, out string error)
        {
            e.Validate();
            var removed = new HashSet<Vector2Int>();
            var affected = new HashSet<Vector2Int>();

            foreach (var p in e.area.allPositionsWithin)
            {
                if (!e.grid.Contains(p)) continue;
                var cell = e.grid[p.x, p.y];
                if (cell.Floor is not FloorTileSo && cell.Floor is not WallTileSo) continue;
                removed.Add(p);
                affected.Add(p);
                if (cell.Floor is WallTileSo && e.grid.Contains(p + Vector2Int.up))
                    affected.Add(p + Vector2Int.up);
            }

            if (removed.Count == 0)
            {
                error = "The brush contains no removable floors or walls.";
                return null;
            }

            var cmd = new CellCommand(e.builder);

            foreach (var p in affected)
            {
                var cell = e.grid[p.x, p.y];
                var after = (GridCell)cell.Clone();
                if (removed.Contains(p)) after.Floor = null;
                var below = p + Vector2Int.down;
                var wallBelow = e.grid.Contains(below) && !removed.Contains(below)
                    ? e.grid[below.x, below.y].Floor as WallTileSo
                    : null;

                if (after.Floor is WallTileSo wall)
                    after.BaseTile = wallBelow ? wall.Roof : wall.Wall;
                else if (wallBelow)
                {
                    after.Floor = null;
                    after.BaseTile = wallBelow.Roof;
                }
                else if (after.Floor is FloorTileSo floor)
                    after.BaseTile = floor.Floor;
                else
                    after.BaseTile = cell.DefaultTile;

                if (cell.Floor == after.Floor && cell.BaseTile == after.BaseTile) continue;

                if (cell.Furniture || cell.Decoration.Any(t => t))
                {
                    error = "Remove furniture and toppings from affected cells first.";
                    return null;
                }

                cmd.before.Add(cell.Clone());
                cmd.after.Add(after);
            }

            cmd.cost.AddItem(e.builder.Material, -removed.Count);

            error = null;
            return cmd;
        }
    }
}