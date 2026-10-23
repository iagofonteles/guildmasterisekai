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
        int Layer { get; }
        void IDatabaseItemInternal.SetIndex(int i) { }
        int IDatabaseItem.Index => throw new InvalidOperationException();
    }

    public interface ITileEntry : ITile
    {
        TileBase Tile { get; }
        ITileEntry Roof => null;
        bool CanReplace(ITileEntry other);
    }

    public interface IDecorationEntry : ITile
    {
        GameObject Prefab { get; }
    }
}