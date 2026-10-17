using System.IO;
using Drafts.Database;
using GuildMasterIsekai.Systems;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GuildMasterIsekai.TileGrid
{
    public interface IGridSerializer
    {
        void WriteTile(BinaryWriter writer, TileBase tile);
        void WriteItem(BinaryWriter writer, ITileSo item);
        TileBase ReadTile(BinaryReader reader);
        T ReadItem<T>(BinaryReader reader);
        BinaryWriter GetWriter(string key);
        BinaryReader GetReader(string key);
    }

    public class GameGridSerializer : IGridSerializer
    {
        public void WriteTile(BinaryWriter writer, TileBase tile)
        {
            if (tile == null)
            {
                writer.Write(0);
                return;
            }

            if (tile is not ITileEntry e)
            {
                Debug.LogError("Not ITileEntry " + tile.name, tile);
                writer.Write(0);
                return;
            }

            writer.Write(e.Id);
        }

        public void WriteItem(BinaryWriter writer, ITileSo item)
        {
            writer.Write(item?.Id ?? 0);
        }

        public TileBase ReadTile(BinaryReader reader)
        {
            return Game.Tiles.Find<TileBase>(reader.ReadUInt32());
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