using System.Collections.Generic;
using System.IO;
using Drafts.Database;
using Drafts.SaveData;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GuildMasterIsekai.Systems
{
    [CreateAssetMenu(menuName = "Game/Database")]
    public class Database : DatabaseSO<DatabaseEntrySo> { }

    public static class DatabaseExtensions
    {
        public static void WriteItem<T>(this BinaryWriter w, T item)
            where T : DatabaseEntrySo => w.Write(item?.Id ?? 0);

        public static T ReadItem<T>(this BinaryReader r)
            where T : DatabaseEntrySo => Game.Database.Find<T>(r.ReadUInt32());

        public static void WriteItemCollection<T>(this BinaryWriter w, IReadOnlyCollection<T> col)
            where T : DatabaseEntrySo => w.WriteCollection(WriteItem, col);

        public static IEnumerable<T> ReadItemCollection<T>(this BinaryReader r)
            where T : DatabaseEntrySo => r.ReadCollection(ReadItem<T>);

#if UNITY_EDITOR
        [MenuItem("Game/Refresh Database")]
        public static void RefreshDatabase()
        {
            Game.Database.FetchItemsFromAssets();
            Game.Tiles.FetchItemsFromAssets<TileBase>();
        }
#endif
    }
}