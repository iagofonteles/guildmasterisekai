using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GuildMasterIsekai.TileGrid
{
    /// <summary>Authoritative grid snapshot. Loading replaces state only after full validation.</summary>
    public sealed partial class GridData
    {
        private const int Version = 0;
        private GridCell[,] _cells;
        public BoundsInt Bounds { get; private set; }
        public RectInt Indexes { get; private set; }
        public Vector3 TileSize { get; private set; }
        public HashSet<GridCell> Modified { get; } = new();

        public int Width => _cells.GetLength(0);
        public int Height => _cells.GetLength(1);
        public IGridCell this[int x, int y] => _cells[x, y];

        public GridData(Tilemap source)
        {
            if (!source) throw new ArgumentNullException(nameof(source));
            var b = Bounds = source.cellBounds;
            Indexes = new(0, 0, b.size.x, b.size.y);
            TileSize = source.cellSize;
            _cells = new GridCell[b.size.x, b.size.y];

            foreach (var p in b.allPositionsWithin)
            {
                var tile = source.GetTile(p);
                var key = new Vector2Int(p.x - b.x, p.y - b.y);
                var cell = new GridCell
                {
                    Grid = this,
                    Key = key,
                    Position = source.CellToWorld(p),
                    DefaultTile = tile,
                    BaseTile = tile
                };
                _cells[key.x, key.y] = cell;
                Modified.Add(cell);
            }
        }

        private GridData(GridData src)
        {
            Bounds = src.Bounds;
            Indexes = src.Indexes;
            _cells = new GridCell[src.Width, src.Height];

            foreach (var p in Indexes.allPositionsWithin)
            {
                var cell = src._cells[p.x, p.y].Clone(this);
                _cells[p.x, p.y] = cell;
                Modified.Add(cell);
            }
        }

        internal event Action<IGridCell, IGridCell> StateChanging;

        public bool Contains(Vector2Int position) => Indexes.Contains(position);

        internal void ApplyState(IGridCell state)
        {
            var cell = _cells[state.Key.x, state.Key.y];
            StateChanging?.Invoke(cell, state);
            cell.CopyState(state);
            Modified.Add(cell);
        }
    }
}