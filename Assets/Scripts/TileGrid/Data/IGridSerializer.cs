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
            writer.Write(color != null);
            if (color == null) return;
            writer.Write(color.Id);
            writer.Write(color.Name ?? "");
            writer.Write(color.Region1Ramp);
            writer.Write(color.Region2Ramp);
            writer.Write(color.Region3Ramp);
            writer.Write(color.Region4Ramp);
        }

        BuildColor ReadColor(BinaryReader reader) => reader.ReadBoolean()
            ? new BuildColor(
                reader.ReadString(), reader.ReadUInt32(),
                reader.ReadByte(), reader.ReadByte(),
                reader.ReadByte(), reader.ReadByte())
            : null;
    }
}