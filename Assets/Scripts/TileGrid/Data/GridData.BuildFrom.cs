using System;
using GuildMasterIsekai.Systems;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GuildMasterIsekai.TileGrid
{
    public sealed partial class GridData
    {
        public void BuildFrom(Tilemap defMap, Tilemap baseMap, Tilemap furniture)
        {
            if (!defMap) throw new ArgumentNullException(nameof(defMap));
            var b = Bounds = defMap.cellBounds;
            Indexes = new(0, 0, b.size.x, b.size.y);
            TileSize = defMap.cellSize;
            _cells = new GridCell[b.size.x, b.size.y];

            foreach (var p in b.allPositionsWithin)
            {
                var entryD = defMap.GetTile(p) as ITileEntry;
                //var entryB = baseMap.GetTile(p) as ITileEntry;
                //var entryF = furniture.GetTile(p) as ITileEntry;
                
                var key = new Vector2Int(p.x - b.x, p.y - b.y);
                var cell = new GridCell
                {
                    Grid = this,
                    Key = key,
                    Position = defMap.CellToWorld(p),
                    DefaultTile = entryD,
                    BaseTile = entryD,
                };
                _cells[key.x, key.y] = cell;
                Modified.Add(cell);
            }
        }
    }
}