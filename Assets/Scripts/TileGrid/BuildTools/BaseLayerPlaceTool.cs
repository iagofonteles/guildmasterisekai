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
            e.Validate();

            if (e.item is not BaseTileSo tile)
            {
                error = "item is not a base tile";
                return null;
            }

            var cmd = new CellCommand(e.builder);

            if (tile is FloorTileSo f)
                PopulateFloor(cmd, e, f);

            if (tile is WallTileSo w)
                PopulateWall(cmd, e, w);

            if (!e.bag.CanPay(cmd.cost))
            {
                error = "not enough items";
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
                //TODO brush area should be trimmed before used
                //if (!grid.Contains(p)) continue;

                var cell = e.grid[p.x, p.y];
                if (cell.Floor == tile) continue;
                if (cell.Floor && cell.Floor is not FloorTileSo) continue;

                var after = (GridCell)cell.Clone();
                after.BaseTile = tile.Floor;
                after.Floor = tile;
                cmd.after.Add(after);
                cmd.before.Add(cell.Clone());

                if (!cell.Floor) cost++;
            }

            cmd.cost.AddItem(e.builder.Material, cost);
        }

        // wall can override floor, other wall or null 
        private static void PopulateWall(CellCommand cmd, OperationArgs e, WallTileSo tile)
        {
            var cost = 0;

            foreach (var p in e.area.allPositionsWithin)
            {
                var p1 = p + Vector2Int.up;
                var p2 = p + Vector2Int.down;
                if (!e.grid.Contains(p1)) continue; // no space for the roof
                
                var cell = e.grid[p.x, p.y];
             
                //TODO
                // wall need a roof direct above (y+)
                // if p1.cell.Floor is null or wall, p1.cell.BaseTile become wall
                // if p1.cell.Floor is floor, p1.cell.Floor become null and refund 1
                // if p2.cell.Floor is wall, p.cell.BaseTile become wall
                // if p cell.Floor is floor or wall, costs nothing
                // if p cell.Floor is null, costs 1
                // placement: r- roof, w-wall, x-null or floor
                // RX
                // RR
                // WW
                // XX
            }

            cmd.cost.AddItem(e.builder.Material, cost);
        }
    }
}