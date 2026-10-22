using System.Collections.Generic;
using TileGrid.Tiles;
using UnityEngine;

namespace TileGrid.Data
{
    public interface IGridCell
    {
        GridData Grid { get; }
        Vector2Int Key { get; }
        Vector3 Position { get; }
        byte Rotation { get; }
        IReadOnlyList<ITileEntry> Tiles { get; }
        IReadOnlyList<Color> Colors { get; }
        IReadOnlyList<CellTopping> Decoration { get; }
        GridCell Clone();
    }

    public sealed class GridCell : IGridCell
    {
        public GridData Grid { get; set; }
        public Vector2Int Key { get; set; }
        public Vector3 Position { get; set; }
        public byte Rotation { get; set; }
        public ITileEntry[] Tiles { get; } = new ITileEntry[BuildLayer.Count];
        public Color[] Colors { get; } = new Color[BuildLayer.Count];
        public List<CellTopping> Decoration { get; } = new();

        IReadOnlyList<ITileEntry> IGridCell.Tiles => Tiles;
        IReadOnlyList<Color> IGridCell.Colors => Colors;
        IReadOnlyList<CellTopping> IGridCell.Decoration => Decoration;

        public GridCell Clone()
        {
            var clone = new GridCell
            {
                Grid = Grid, Key = Key,
                Position = Position,
            };
            Tiles.CopyTo(clone.Tiles, 0);
            Colors.CopyTo(clone.Colors, 0);
            clone.Decoration.AddRange(Decoration);
            return clone;
        }

        public void CopyState(IGridCell state)
        {
            if (ReferenceEquals(this, state)) return;
            for (var i = 0; i < BuildLayer.Count; i++)
            {
                Tiles[i] = state.Tiles[i];
                Colors[i] = state.Colors[i];
            }
            Decoration.Clear();
            Decoration.AddRange(state.Decoration);
        }
    }
}
