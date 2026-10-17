using System;
using System.Collections.Generic;
using Drafts.Database;


namespace GuildMasterIsekai.Systems
{
    public interface ITileEntry : IDatabaseItemInternal
    {
        int IDatabaseItem.Index => throw new InvalidOperationException();
        void IDatabaseItemInternal.SetIndex(int i) => throw new InvalidOperationException();
    }

    public interface ITileSo
    {
        IEnumerable<ITileEntry> GetEntries();
    }
}