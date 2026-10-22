using System;
using System.IO;
using TileGrid.Build;
using TileGrid.Tiles;
using UnityEngine;

namespace TileGrid.Data
{
    public interface IGridSerializer
    {
        void WriteTile(BinaryWriter writer, ITile tile);
        ITile ReadTile(BinaryReader reader);
        bool FileExists(string key);
        BinaryWriter GetWriter(string key);
        BinaryReader GetReader(string key);

        void WriteSingle(BinaryWriter writer, float value) => writer.Write(value);
        float ReadSingle(BinaryReader reader) => reader.ReadSingle();

        void WriteVector2(BinaryWriter writer, Vector2 value)
        {
            WriteSingle(writer, value.x);
            WriteSingle(writer, value.y);
        }

        Vector2 ReadVector2(BinaryReader reader) => new(
            ReadSingle(reader),
            ReadSingle(reader));

        void WriteColor(BinaryWriter writer, BuildColor color)
        {
            throw new NotImplementedException();
            // WriteSingle(writer, color.r);
            // WriteSingle(writer, color.g);
            // WriteSingle(writer, color.b);
            // WriteSingle(writer, color.a);
        }

        BuildColor ReadColor(BinaryReader reader) => throw new NotImplementedException();
    }
}