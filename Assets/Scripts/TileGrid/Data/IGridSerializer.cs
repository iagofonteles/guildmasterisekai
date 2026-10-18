using System.IO;
using GuildMasterIsekai.Systems;

namespace GuildMasterIsekai.TileGrid
{
    public interface IGridSerializer
    {
        void WriteTile(BinaryWriter writer, ITileEntry tile);
        void WriteItem(BinaryWriter writer, ITileSo item);
        ITileEntry ReadTile(BinaryReader reader);
        T ReadItem<T>(BinaryReader reader);
        BinaryWriter GetWriter(string key);
        BinaryReader GetReader(string key);
    }
}