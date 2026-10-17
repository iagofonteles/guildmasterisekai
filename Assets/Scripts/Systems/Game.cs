using System.Collections.Generic;
using Drafts.Database;
using Drafts.SaveData;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace GuildMasterIsekai.Systems
{
    public static class Game
    {
        private static Database _database;
        private static RuntimeDatabase<ITileEntry> _tiles;
        private static UIntGenerator _id;
        private static SaveManager _save;

        public static Database Database => Load(ref _database);
        public static RuntimeDatabase<ITileEntry> Tiles => _tiles ??= new(GetTileEntries());
        public static SaveManager Save { get; } = new("Save", "Default", new SaveFileParser());
        public static UIntGenerator Id { get; } = new();

        private static T Load<T>(ref T v, string path = null) where T : Object
        {
            if (v) return v;
            return v = Resources.Load<T>(path ?? typeof(T).Name);
        }

        [RuntimeInitializeOnLoadMethod]
        public static void Reset()
        {
            Save.Load("Default");
        }

        public static Sprite LoadSprite(string path) =>
            Addressables.LoadAssetAsync<Sprite>($"Sprites/{path}").WaitForCompletion();

        private static IEnumerable<ITileEntry> GetTileEntries()
        {
            foreach (var tileSo in Database.GetAll<ITileSo>())
            foreach (var entry in tileSo.GetEntries())
                yield return entry;
        }
    }
}