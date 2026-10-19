using System.Collections.Generic;
using GuildMasterIsekai.Systems;

namespace GuildMasterIsekai.TileGrid
{
    public abstract class BaseTileSo : DatabaseEntrySo, ITileSo, ITileProvider
    {
        public bool Solid => true;
        public abstract IEnumerable<ITileEntry> GetEntries();
        public abstract ITileEntry Tile { get; }
    }
}