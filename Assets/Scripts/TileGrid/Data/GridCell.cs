using System.Collections.Generic;
using GuildMasterIsekai.Systems;
using UnityEngine;

namespace GuildMasterIsekai.TileGrid
{
    public interface IGridCell
    {
        GridData Grid { get; }
        Vector2Int Key { get; }
        Vector3 Position { get; }
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
        public ITileEntry[] Tiles { get; } = new ITileEntry[GridData.LayerCount];
        public Color[] Colors { get; } = new Color[GridData.LayerCount];
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
            var s = (GridCell)state;
            s.Tiles.CopyTo(Tiles, 0);
            s.Colors.CopyTo(Colors, 0);
            Decoration.Clear();
            Decoration.AddRange(s.Decoration);
        }
    }
}