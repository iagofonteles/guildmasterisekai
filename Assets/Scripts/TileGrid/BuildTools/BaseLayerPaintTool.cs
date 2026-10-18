using System;
using System.Collections.Generic;
using Drafts.Inventory;
using UnityEngine.Scripting;

namespace GuildMasterIsekai.TileGrid
{
    [Serializable, Preserve]
    public sealed class BaseLayerPaintTool : IBuildTool
    {
        public GridBuilder Builder { get; set; }

        public IEnumerable<ISlot> GetItems() => Builder.Bag.GetItems<BaseTileSo>();

        public IBuildCommand CreateCommand(OperationArgs e, out string error)
        {
            e.Validate();
            var cmd = new CellCommand(e.builder);

            foreach (var p in e.area.allPositionsWithin)
            {
                if (!e.grid.Contains(p)) continue;
                var cell = e.grid[p.x, p.y];
                if (cell.BaseTile is not DbAutoTile current) continue;
                var after = (GridCell)cell.Clone();

                if (e.item is FloorTileSo floor && current.Type == AutoTileType.Floor)
                {
                    if (!floor.Floor) continue;
                    after.BaseTile = floor.Floor;
                    if (cell.Floor is FloorTileSo) after.Floor = floor;
                }
                else if (e.item is WallTileSo wall &&
                         (current.Type == AutoTileType.Wall || current.Type == AutoTileType.Roof))
                {
                    var variant = current.Type == AutoTileType.Wall ? wall.Wall : wall.Roof;
                    if (!variant) continue;
                    after.BaseTile = variant;
                    if (cell.Floor is WallTileSo) after.Floor = wall;
                }
                else continue;

                if (cell.BaseTile == after.BaseTile && cell.Floor == after.Floor) continue;
                cmd.before.Add(cell.Clone());
                cmd.after.Add(after);
            }

            if (cmd.after.Count == 0)
            {
                error = "The brush contains no matching paintable changes.";
                return null;
            }

            error = null;
            return cmd;
        }
    }
}