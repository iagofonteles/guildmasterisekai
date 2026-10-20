using System;
using Drafts.Database;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TileGrid.Tiles
{
    public interface ITile : IDatabaseItemInternal
    {
        public Sprite Icon { get; }
        public string DisplayName { get; }
        public string Description { get; }
        BuildLayerMask LayerMask { get; }
        int IDatabaseItem.Index => throw new InvalidOperationException();
        void IDatabaseItemInternal.SetIndex(int i) => throw new InvalidOperationException();
    }

    public interface ITileEntry : ITile
    {
        TileBase Tile { get; }
    }

    public interface IRoofedTile
    {
        ITileEntry Roof { get; }
    }

    public interface IDecorationEntry : ITile
    {
        GameObject Prefab { get; }
    }
}