using System.IO;
using TileGrid.Tiles;
using UnityEngine;

namespace TileGrid.Data
{
    public struct CellTopping
    {
        public IDecorationEntry item;
        public Vector2 position;
        public Color color;

        public void Save(IGridSerializer serializer, BinaryWriter writer)
        {
            serializer.WriteTile(writer, item);
            serializer.WriteVector2(writer, position);
            serializer.WriteColor(writer, color);
        }

        public static CellTopping Load(IGridSerializer serializer, BinaryReader reader) => new()
        {
            item = (IDecorationEntry)serializer.ReadTile(reader),
            position = serializer.ReadVector2(reader),
            color = serializer.ReadColor(reader),
        };
    }
}