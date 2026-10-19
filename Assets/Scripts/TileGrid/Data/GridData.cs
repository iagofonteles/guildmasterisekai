using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GuildMasterIsekai.TileGrid
{
    /// <summary>Authoritative grid snapshot. Loading replaces state only after full validation.</summary>
    public sealed partial class GridData
    {
        public const int LayerCount = 6;

        private const int Version = 0;
        private GridCell[,] _cells;
        public BoundsInt Bounds { get; private set; }
        public RectInt Indexes { get; private set; }
        public Vector3 TileSize { get; private set; }
        public HashSet<GridCell> Modified { get; } = new();

        public int Width => _cells.GetLength(0);
        public int Height => _cells.GetLength(1);
        public IGridCell this[int x, int y] => _cells[x, y];

        public GridData(Tilemap[] layers) => BuildFrom(layers);
        public GridData(IGridSerializer serializer, string key) => Load(serializer, key);
        public GridData(GridData src)
        {
            Bounds = src.Bounds;
            Indexes = src.Indexes;
            _cells = new GridCell[src.Width, src.Height];

            foreach (var p in Indexes.allPositionsWithin)
            {
                var cell = src._cells[p.x, p.y].Clone();
                cell.Grid = this;
                _cells[p.x, p.y] = cell;
                Modified.Add(cell);
            }
        }

        public bool Contains(Vector2Int position) => Indexes.Contains(position);

        internal void ApplyState(IGridCell state)
        {
            var cell = _cells[state.Key.x, state.Key.y];
            cell.CopyState(state);
            Modified.Add(cell);
        }
    }
}