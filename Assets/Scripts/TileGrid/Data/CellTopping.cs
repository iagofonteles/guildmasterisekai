using System.IO;
using GuildMasterIsekai.Systems;
using UnityEngine;

namespace GuildMasterIsekai.TileGrid
{
    public struct CellTopping
    {
        public IPrefabTile item;
        public Vector2 position;
        public Color color;

        public void Save(IGridSerializer serializer, BinaryWriter writer)
        {
            serializer.WriteItem(writer, item);
            serializer.WriteVector2(writer, position);
            serializer.WriteColor(writer, color);
        }

        public static CellTopping Load(IGridSerializer serializer, BinaryReader reader) => new()
        {
            item = serializer.ReadItem<IPrefabTile>(reader),
            position = serializer.ReadVector2(reader),
            color = serializer.ReadColor(reader),
        };
    }
}