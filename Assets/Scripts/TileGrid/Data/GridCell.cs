using System.Collections.Generic;
using GuildMasterIsekai.Systems;
using UnityEngine;

namespace GuildMasterIsekai.TileGrid
{
    public interface IGridCell
    {
        Vector3 Position { get; }
        Vector2Int Key { get; }
        ITileEntry DefaultTile { get; }
        ITileEntry BaseTile { get; }
        BaseTileSo Floor { get; }
        FurnitureTileSo Furniture { get; }
        public Color BaseColor { get; }
        public Color FurnitureColor { get; }
        IReadOnlyList<ToppingTileSo> Toppings { get; }
        IGridCell Clone();
    }

    public sealed class GridCell : IGridCell
    {
        public GridData Grid { get; internal set; }
        public Vector2Int Key { get; internal set; }
        public Vector3 Position { get; internal set; }
        public ITileEntry DefaultTile { get; internal set; }
        public ITileEntry BaseTile { get; internal set; }
        public BaseTileSo Floor { get; internal set; }
        public FurnitureTileSo Furniture { get; internal set; }
        public Color BaseColor { get; internal set; }
        public Color FurnitureColor { get; internal set; }
        internal List<ToppingTileSo> slots = new();

        public IReadOnlyList<ToppingTileSo> Toppings => slots;

        IGridCell IGridCell.Clone() => Clone(Grid);

        internal GridCell Clone(GridData grid) => new()
        {
            Grid = grid ?? Grid,
            Key = Key,
            Position = Position,
            DefaultTile = DefaultTile,
            BaseTile = BaseTile,
            Floor = Floor,
            Furniture = Furniture,
            BaseColor = BaseColor,
            FurnitureColor = FurnitureColor,
            slots = new(slots)
        };

        internal void CopyState(IGridCell state)
        {
            DefaultTile = state.DefaultTile;
            BaseTile = state.BaseTile;
            Floor = state.Floor;
            Furniture = state.Furniture;
            BaseColor = state.BaseColor;
            FurnitureColor = state.FurnitureColor;
            slots.Clear();
            slots.AddRange(state.Toppings);
        }
    }
}
