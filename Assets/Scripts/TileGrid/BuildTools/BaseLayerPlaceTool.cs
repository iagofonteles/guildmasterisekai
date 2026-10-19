using System;
using System.Collections.Generic;
using Drafts.Inventory;
using TileGrid.Tiles;
using UnityEngine;
using UnityEngine.Scripting;

namespace GuildMasterIsekai.TileGrid
{
    [Serializable, Preserve]
    public sealed class BaseLayerPlaceTool : IBuildTool
    {
        public IEnumerable<ISlot> GetItems(IBuildBag bag) => bag.GetItems<BaseTileSo>();

        public IBuildCommand CreateCommand(OperationArgs e, out string error)
        {
            e.Validate();

            if (e.item is not BaseTileSo tile)
            {
                error = "item is not a base tile";
                return null;
            }

            var cmd = new CellCommand(e.builder);
            if (tile is FloorTileSo f) PopulateFloor(cmd, e, f);
            if (tile is WallTileSo w) PopulateWall(cmd, e, w);

            if (cmd.after.Count == 0)
            {
                error = "no change";
                return null;
            }

            error = null;
            return cmd;
        }

        // floor can only override other floor or null 
        private static void PopulateFloor(CellCommand cmd, OperationArgs e, FloorTileSo tile)
        {
            var cost = 0;

            foreach (var p in e.area.allPositionsWithin)
            {
                if (!e.grid.Contains(p)) continue;

                var cell = e.grid[p.x, p.y];
                if (cell.Floor == tile) continue;
                if (cell.Floor && cell.Floor is not FloorTileSo) continue;
                if (cell.BaseTile != null && cell.BaseTile is not DbAutoTile { Type: AutoTileType.Floor })
                    continue;

                var after = (GridCell)cell.Clone();
                after.BaseTile = tile.Floor;
                after.Floor = tile;
                cmd.after.Add(after);
                cmd.before.Add(cell.Clone());

                if (!cell.Floor) cost++;
            }

            cmd.cost.AddItem(e.builder.Material, cost);
        }

        // Wall ownership is separate from the visible wall/roof variant.
        private static void PopulateWall(CellCommand cmd, OperationArgs e, WallTileSo tile)
        {
            var anchors = new HashSet<Vector2Int>();
            var changes = new Dictionary<Vector2Int, GridCell>();
            foreach (var p in e.area.allPositionsWithin)
            {
                if (!e.grid.Contains(p) || !e.grid.Contains(p + Vector2Int.up)) continue;
                var cell = e.grid[p.x, p.y];
                if (cell.Floor && cell.Floor is not FloorTileSo && cell.Floor is not WallTileSo) continue;
                anchors.Add(p);
                var after = (GridCell)cell.Clone();
                after.Floor = tile;
                changes[p] = after;
            }

            foreach (var p in anchors)
            {
                var below = p + Vector2Int.down;
                var hasWallBelow = anchors.Contains(below) ||
                    e.grid.Contains(below) && e.grid[below.x, below.y].Floor is WallTileSo;
                changes[p].BaseTile = hasWallBelow ? tile.Roof : tile.Wall;
            }

            foreach (var p in anchors)
            {
                var above = p + Vector2Int.up;
                if (!changes.TryGetValue(above, out var roof))
                {
                    roof = (GridCell)e.grid[above.x, above.y].Clone();
                    changes.Add(above, roof);
                }
                if (roof.Floor is FloorTileSo) roof.Floor = null;
                roof.BaseTile = roof.Floor is WallTileSo owner ? owner.Roof : tile.Roof;
            }

            var cost = 0;
            foreach (var pair in changes)
            {
                var cell = e.grid[pair.Key.x, pair.Key.y];
                var after = pair.Value;
                if (cell.Floor == after.Floor && cell.BaseTile == after.BaseTile) continue;
                cmd.before.Add(cell.Clone());
                cmd.after.Add(after);
                if (!cell.Floor && after.Floor) cost++;
                else if (cell.Floor && !after.Floor) cost--;
            }
            cmd.cost.AddItem(e.builder.Material, cost);
        }
    }
}