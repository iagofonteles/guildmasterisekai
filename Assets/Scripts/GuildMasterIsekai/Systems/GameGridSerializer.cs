using System.IO;
using GuildMasterIsekai.Systems;
using TileGrid.Data;
using TileGrid.Tiles;

namespace GuildMasterIsekai.TileGrid
{
    public class GameGridSerializer : IGridSerializer
    {
        public void WriteTile(BinaryWriter writer, ITile tile)
        {
            writer.Write(tile?.Id ?? 0);
        }

        public ITile ReadTile(BinaryReader reader)
        {
            return Game.Tiles.Find(reader.ReadUInt32());
        }

        public BinaryWriter GetWriter(string key)
        {
            var mngr = Game.SaveMngr;
            key ??= "GridData";
            key = Path.Combine(mngr.Root, mngr.Folder, key);
            var file = File.Create(key);
            return new BinaryWriter(file);
        }

        public BinaryReader GetReader(string key)
        {
            var mngr = Game.SaveMngr;
            key ??= "GridData";
            key = Path.Combine(mngr.Root, mngr.Folder, key);
            var file = File.OpenRead(key);
            return new BinaryReader(file);
        }
    }
}