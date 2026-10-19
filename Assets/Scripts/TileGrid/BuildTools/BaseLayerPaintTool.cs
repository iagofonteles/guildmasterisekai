using System;
using System.Collections.Generic;
using Drafts.Inventory;
using UnityEngine;
using UnityEngine.Scripting;
using static GuildMasterIsekai.TileGrid.AutoTileType;

namespace GuildMasterIsekai.TileGrid
{
    [Serializable, Preserve]
    public sealed class BaseLayerPaintTool : IBuildTool
    {
        public IEnumerable<ISlot> GetItems(IBuildBag bag) => bag.GetItems<BaseTileSo>();

        public IBuildCommand CreateCommand(OperationArgs e, out string error)
        {
            e.Validate();
            var cmd = new CellCommand(e.builder);
            var changes = new Dictionary<Vector2Int, GridCell>();

            void PaintWall(Vector2Int position, WallTileSo wall, AutoTileType type)
            {
                if (!e.grid.Contains(position)) return;
                var cell = e.grid[position.x, position.y];
                if (cell.BaseTile is not DbAutoTile current || current.Type != type) return;
                var variant = type == Wall ? wall.Wall : wall.Roof;
                if (!variant) return;
                if (!changes.TryGetValue(position, out var after))
                    after = (GridCell)cell.Clone();
                after.BaseTile = variant;
                if (cell.Floor is WallTileSo) after.Floor = wall;
                changes[position] = after;
            }

            foreach (var p in e.area.allPositionsWithin)
            {
                if (!e.grid.Contains(p)) continue;
                var cell = e.grid[p.x, p.y];
                if (cell.BaseTile is not DbAutoTile current) continue;

                if (e.item is FloorTileSo floor && current.Type == Floor)
                {
                    // Painting must not turn an unowned/default floor into a painted floor.
                    if (!cell.Floor || !floor.Floor) continue;
                    var after = (GridCell)cell.Clone();
                    after.BaseTile = floor.Floor;
                    if (cell.Floor is FloorTileSo) after.Floor = floor;
                    changes[p] = after;
                }
                else if (e.item is WallTileSo wall && current.Type is Wall or Roof)
                {
                    PaintWall(p, wall, current.Type);
                    var neighbour = p + (current.Type == Wall ? Vector2Int.up : Vector2Int.down);
                    PaintWall(neighbour, wall, current.Type == Wall ? Roof : Wall);
                }
            }

            foreach (var pair in changes)
            {
                var cell = e.grid[pair.Key.x, pair.Key.y];
                var after = pair.Value;
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
/*
Execution report (2026-10-18)
Summary: Implemented the floor-ownership and paired wall/roof painting comments.
Completed work: Floor painting skips cells with no Floor owner. Painting a visible wall also paints an existing roof immediately above; painting a visible roof also paints an existing wall immediately below, including neighbours outside the brush. Changes are deduplicated by cell before creating the undoable command. Existing ownership metadata is updated without creating new ownership.
Validation results: dotnet build GuildMasterIsekai.csproj --no-restore --no-dependencies passed with 0 errors and 4 existing unused-event warnings. git diff --check passed. Reviewed grid-boundary guards, neighbour type checks, missing variants, unchanged cells and duplicate selections in source.
Limitations: No Unity runtime tests performed. Missing neighbours or target variants are skipped; painting does not create wall/roof cells or extend through chains of roof cells.
Modified assets / files:
- Assets/Scripts/TileGrid/BuildTools/BaseLayerPaintTool.cs
*/
