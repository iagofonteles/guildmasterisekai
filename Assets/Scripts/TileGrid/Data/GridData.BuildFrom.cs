using System;
using TileGrid.Tiles;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TileGrid.Data
{
    public sealed partial class GridData
    {
        public void BuildFrom(Tilemap[] layers)
        {
            if ((layers?.Length ?? 0) == 0) throw new ArgumentException(nameof(layers));
            var min = Math.Min(BuildLayer.Count, layers.Length);
            for (var l = 0; l < min; l++)
                if (!layers[l]) throw new ArgumentException("Tilemap layers must not be null.", nameof(layers));
            
            var b = Bounds = layers[0].cellBounds;
            Indexes = new(0, 0, b.size.x, b.size.y);
            TileSize = layers[0].cellSize;
            _cells = new GridCell[b.size.x, b.size.y];
            Modified.Clear();

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
                {
                    cell.Tiles[l] = layers[l].GetTile(p) as ITileEntry;
                    cell.Colors[l] = null;
                }

                _cells[key.x, key.y] = cell;
                Modified.Add(cell);
            }
        }
    }
}
