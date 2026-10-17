using System;
using System.Collections.Generic;
using UnityEngine;

namespace GuildMasterIsekai.TileGrid
{
    public sealed class BaseLayerPlaceTool : IBuildTool
    {
        public bool RequiresItem => true;
        public IEnumerable<object> GetCompatibleItems(IBuildBag bag) => bag.GetUnlockedTiles();

        public IBuildCommand CreateCommand(OperationArgs e, out string error)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            if (e.builder == null) throw new ArgumentNullException(nameof(e.builder));
            if (e.grid == null) throw new NullReferenceException(nameof(e.grid));
            if (e.bag == null) throw new NullReferenceException(nameof(e.bag));
            if (!e.builder.Material)
            {
                error = "Assign a building material to GridBuilder.";
                return null;
            }

            if (e.item is not BaseTileSo tile)
            {
                error = "item is not a base tile";
                return null;
            }

            if (!GridEditing.CheckArea(e.grid, e.area, out error)) return null;
            if (!tile || !tile.Floor || !tile.Wall || !tile.Roof ||
                tile.Floor.Type != AutoTileType.Floor || tile.Wall.Type != AutoTileType.Wall ||
                tile.Roof.Type != AutoTileType.Roof)
            {
                error = "Select a base tile with valid floor, wall and roof variants.";
                return null;
            }

            var count = 0;
            foreach (var p in e.area.allPositionsWithin)
                if (!e.grid[p.x, p.y].Floor)
                    count++;

            if (count == 0)
            {
                error = "no change";
                return null;
            }

            var cmd = new CellCommand(e.builder);
            cmd.cost.AddItem(e.builder.Material, count);

            if (!e.bag.CanPay(cmd.cost))
            {
                error = "not enough items";
                return null;
            }

            error = null;
            PopulateCmd(e.grid, e.area, tile, cmd);
            return cmd;
        }

        private static void PopulateCmd(GridData grid, RectInt area, BaseTileSo tile, CellCommand cmd)
        {
            var patterns = new Dictionary<Vector2Int, AutoTileType>();
            var anchors = new HashSet<Vector2Int>();

            foreach (var anchor in area.allPositionsWithin)
            {
                if (!grid.Contains(anchor) || grid[anchor.x, anchor.y].Floor) continue;
                anchors.Add(anchor);

                for (var y = -2; y <= 2; y++)
                for (var x = -1; x <= 1; x++)
                {
                    var p = anchor + new Vector2Int(x, y);
                    if (!grid.Contains(p)) continue;
                    var type = x == 0 && y == 0 ? AutoTileType.Floor :
                        y == -2 || (x == 0 && y == 1) ? AutoTileType.Wall : AutoTileType.Roof;
                    if (patterns.TryGetValue(p, out var previous) && Priority(previous) >= Priority(type))
                        continue;
                    patterns[p] = type;
                }
            }

            foreach (var pair in patterns)
            {
                var p = pair.Key;
                var cell = grid[p.x, p.y];
                // Existing anchors keep their material; only newly selected anchors are added.
                if (cell.Floor) continue;
                var type = pair.Value;
                if (cell.BaseTile is DbAutoTile current && Priority(current.Type) > Priority(type))
                    continue;

                var expectedTile = tile[type];
                var expectedFloor = anchors.Contains(p) ? tile : cell.Floor;
                if (cell.BaseTile == expectedTile && cell.Floor == expectedFloor) continue;

                var after = (GridCell)cell.Clone();
                after.BaseTile = expectedTile;
                after.Floor = expectedFloor;
                cmd.before.Add(cell.Clone());
                cmd.after.Add(after);
            }
        }

        private static int Priority(AutoTileType type) => type switch
        {
            AutoTileType.Floor => 3,
            AutoTileType.Wall => 2,
            AutoTileType.Roof => 1,
            _ => 0
        };
    }
}