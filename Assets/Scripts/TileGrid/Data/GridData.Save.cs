using System.IO;
using GuildMasterIsekai.Systems;
using UnityEngine;

namespace GuildMasterIsekai.TileGrid
{
    public sealed partial class GridData
    {
        public void Save(IGridSerializer serializer, string key)
        {
            using var writer = serializer.GetWriter(key);
            writer.Write((short)Version);
            writer.Write((short)LayerCount);
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

                foreach (var tile in cell.Tiles)
                    serializer.WriteTile(writer, tile);
                foreach (var color in cell.Colors)
                    serializer.WriteColor(writer, color);

                writer.Write(cell.Decoration.Count);
                foreach (var topping in cell.Decoration)
                    topping.Save(serializer, writer);
            }
        }

        public void Load(IGridSerializer serializer, string key)
        {
            using var reader = serializer.GetReader(key);
            if (reader.ReadInt16() != Version) throw new InvalidDataException("Unsupported version.");
            if (reader.ReadInt16() != LayerCount) throw new InvalidDataException("Unsupported layer count.");

            var minX = reader.ReadInt32();
            var minY = reader.ReadInt32();
            var width = reader.ReadInt32();
            var height = reader.ReadInt32();
            var sx = reader.ReadSingle();
            var sy = reader.ReadSingle();

            if (width < 0 || height < 0 || (long)width * height > int.MaxValue ||
                (long)minX + width > int.MaxValue || (long)minY + height > int.MaxValue)
                throw new InvalidDataException("Invalid grid bounds.");
            if (float.IsNaN(sx) || float.IsInfinity(sx) || sx <= 0 ||
                float.IsNaN(sy) || float.IsInfinity(sy) || sy <= 0)
                throw new InvalidDataException("Invalid tile size.");

            var cells = new GridCell[width, height];

            for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
            {
                var cell = new GridCell
                {
                    Grid = this, Key = new(i, j),
                    Position = new(reader.ReadSingle(), reader.ReadSingle(), 0),
                };
                if (float.IsNaN(cell.Position.x) || float.IsInfinity(cell.Position.x) ||
                    float.IsNaN(cell.Position.y) || float.IsInfinity(cell.Position.y))
                    throw new InvalidDataException("Invalid cell position.");

                for (var t = 0; t < LayerCount; t++)
                    cell.Tiles[t] = serializer.ReadTile(reader);
                for (var c = 0; c < LayerCount; c++)
                    cell.Colors[c] = serializer.ReadColor(reader);

                var count = reader.ReadInt32();
                if (count < 0) throw new InvalidDataException("Invalid decoration count.");
                for (var k = 0; k < count; k++)
                    cell.Decoration.Add(CellTopping.Load(serializer, reader));

                cells[i, j] = cell;
            }

            Bounds = new(minX, minY, 0, width, height, 1);
            Indexes = new(0, 0, width, height);
            TileSize = new(sx, sy, 0);
            _cells = cells;
            Modified.Clear();
            foreach (var cell in cells)
                Modified.Add(cell);
        }
    }
}
