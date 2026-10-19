using System.Collections.Generic;
using GuildMasterIsekai.Systems;

namespace GuildMasterIsekai.TileGrid
{
    public interface ITileProvider
    {
        IEnumerable<ITileEntry> GetEntries();
    }
}