using System;
using System.Collections.Generic;
using Drafts.Database;
using UnityEngine.Tilemaps;

namespace GuildMasterIsekai.Systems
{
    public interface ITileEntry : IDatabaseItemInternal
    {
        int IDatabaseItem.Index => throw new InvalidOperationException();
        void IDatabaseItemInternal.SetIndex(int i) => throw new InvalidOperationException();
        TileBase Tile { get; }
    }

    public interface ITileSo : IDatabaseItem
    {
        IEnumerable<ITileEntry> GetEntries();
    }
}