using System;
using GuildMasterIsekai.Systems;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GuildMasterIsekai.TileGrid
{
    public sealed partial class GridData
    {
        public void BuildFrom(Tilemap[] layers)
        {
            if ((layers?.Length ?? 0) == 0) throw new ArgumentException(nameof(layers));
            var b = Bounds = layers[0].cellBounds;
            Indexes = new(0, 0, b.size.x, b.size.y);
            TileSize = layers[0].cellSize;
            _cells = new GridCell[b.size.x, b.size.y];

            var min = Math.Min(LayerCount, layers.Length);

            foreach (var p in b.allPositionsWithin)
            {
                var key = new Vector2Int(p.x - b.x, p.y - b.y);
                var cell = new GridCell
                {
                    Grid = this,
                    Key = key,
                    Position = layers[0].CellToWorld(p),
                };

                for (var l = 0; l < min; l++)
                    cell.Tiles[l] = layers[l].GetTile(p) as ITileEntry;

                _cells[key.x, key.y] = cell;
                Modified.Add(cell);
            }
        }
    }
}