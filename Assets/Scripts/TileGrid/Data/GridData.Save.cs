using System.IO;
using UnityEngine;

namespace GuildMasterIsekai.TileGrid
{
    public sealed partial class GridData
    {
        public void Save(IGridSerializer serializer, string key)
        {
            using var writer = serializer.GetWriter(key);
            writer.Write(Version);
            writer.Write(Bounds.position.x);
            writer.Write(Bounds.position.y);
            writer.Write(Bounds.size.x);
            writer.Write(Bounds.size.y);
            writer.Write(TileSize.x);
            writer.Write(TileSize.y);

            for (var i = 0; i < _cells.GetLength(0); i++)
            for (var j = 0; j < _cells.GetLength(1); j++)
            {
                var cell = _cells[i, j];
                writer.Write(cell.Position.x);
                writer.Write(cell.Position.y);
                serializer.WriteTile(writer, cell.DefaultTile);
                serializer.WriteTile(writer, cell.BaseTile);
                serializer.WriteItem(writer, cell.Floor);
                serializer.WriteItem(writer, cell.Furniture);
                WriteColor(writer, cell.BaseColor);
                WriteColor(writer, cell.FurnitureColor);
                writer.Write(cell.slots.Count);
                foreach (var topping in cell.slots)
                    writer.Write(topping?.Id ?? 0);
            }
        }

        public void Load(IGridSerializer serializer, string key)
        {
            using var reader = serializer.GetReader(key);
            var version = reader.ReadInt32();
            if (version < 0 || version > Version) throw new InvalidDataException("Unsupported grid save version.");

            var minX = reader.ReadInt32();
            var minY = reader.ReadInt32();
            var width = reader.ReadInt32();
            var height = reader.ReadInt32();
            var sx = reader.ReadSingle();
            var sy = reader.ReadSingle();

            Bounds = new(minX, minY, 0, width, height, 1);
            Indexes = new(0, 0, width, height);
            TileSize = new(sx, sy, 0);
            _cells = new GridCell[width, height];
            Modified.Clear();

            for (var i = 0; i < _cells.GetLength(0); i++)
            for (var j = 0; j < _cells.GetLength(1); j++)
            {
                var cell = new GridCell
                {
                    Grid = this,
                    Key = new(i, j),
                    Position = new(reader.ReadSingle(), reader.ReadSingle(), 0),
                    DefaultTile = serializer.ReadTile(reader),
                    BaseTile = serializer.ReadTile(reader),
                    Floor = serializer.ReadItem<BaseTileSo>(reader),
                    Furniture = serializer.ReadItem<FurnitureTileSo>(reader),
                    BaseColor = ReadColor(reader),
                    FurnitureColor = ReadColor(reader),
                };

                var count = reader.ReadInt32();
                for (var k = 0; k < count; k++)
                    cell.slots.Add(serializer.ReadItem<ToppingTileSo>(reader));

                _cells[i, j] = cell;
                Modified.Add(cell);
            }
        }

        private static void WriteColor(BinaryWriter writer, Color color)
        {
            writer.Write(color.r);
            writer.Write(color.g);
            writer.Write(color.b);
            writer.Write(color.a);
        }

        private static Color ReadColor(BinaryReader reader) => new(
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle());
    }
}