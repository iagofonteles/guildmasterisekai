using System.Collections.Generic;
using System.IO;
using Drafts.Database;
using Drafts.SaveData;
using GuildMasterIsekai.TileGrid;
using TileGrid.Tiles;
using UnityEngine;

namespace GuildMasterIsekai.Systems
{
    [CreateAssetMenu(menuName = "Game/TileDatabase")]
    public class TileDatabase : FlexibleDatabaseSO<ITile>
    {
        
    }

    public static class TileDatabaseExtensions
    {
        public static void WriteTile<T>(this BinaryWriter w, T item)
            where T : ITileEntry => w.Write(item?.Id ?? 0);

        public static T ReadTile<T>(this BinaryReader r)
            where T : ITileEntry => Game.Tiles.Find<T>(r.ReadUInt32());

        public static void WriteTileCollection<T>(this BinaryWriter w, IReadOnlyCollection<T> col)
            where T : ITileEntry => w.WriteCollection(WriteTile, col);

        public static IEnumerable<T> ReadTileCollection<T>(this BinaryReader r)
            where T : ITileEntry => r.ReadCollection(ReadTile<T>);
    }
}