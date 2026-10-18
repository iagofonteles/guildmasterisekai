using System.IO;
using GuildMasterIsekai.Systems;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    public class GameGridSerializer : IGridSerializer
    {
        public void WriteTile(BinaryWriter writer, ITileEntry tile)
        {
            writer.Write(tile?.Id ?? 0);
        }

        public void WriteItem(BinaryWriter writer, ITileSo item)
        {
            writer.Write(item?.Id ?? 0);
        }

        public ITileEntry ReadTile(BinaryReader reader)
        {
            return Game.Tiles.Find(reader.ReadUInt32());
        }

        public T ReadItem<T>(BinaryReader reader)
        {
            return Game.Database.Find<T>(reader.ReadUInt32());
        }

        public BinaryWriter GetWriter(string key)
        {
            var mngr = Game.SaveMngr;
            key = Path.Combine(mngr.Root, mngr.Folder, key);
            var file = File.Create(key);
            return new BinaryWriter(file);
        }

        public BinaryReader GetReader(string key)
        {
            var mngr = Game.SaveMngr;
            key = Path.Combine(mngr.Root, mngr.Folder, key);
            var file = File.OpenRead(key);
            return new BinaryReader(file);
        }
    }
}