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

        public bool FileExists(string key) => File.Exists(ResolveKey(key));

        public BinaryWriter GetWriter(string key)
        {
            key = ResolveKey(key);
            var dir = Path.GetDirectoryName(key);
            Directory.CreateDirectory(dir!);
            var file = File.Create(key);
            return new BinaryWriter(file);
        }

        public BinaryReader GetReader(string key)
        {
            key = ResolveKey(key);
            var file = File.OpenRead(key);
            return new BinaryReader(file);
        }

        private static string ResolveKey(string key)
        {
            var mngr = Game.SaveMngr;
            key ??= "GridData";
            return Path.Combine(mngr.Root, mngr.Folder, key, ".grid");
        }
    }
}