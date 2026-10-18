using System.Collections.Generic;
using GuildMasterIsekai.Systems;

namespace GuildMasterIsekai.TileGrid
{
    public abstract class BaseTileSo : DatabaseEntrySo, ITileSo
    {
        public bool Solid => true;
        public abstract IEnumerable<ITileEntry> GetEntries();
    }
}