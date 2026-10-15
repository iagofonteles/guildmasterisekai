using System.Collections.Generic;
using Drafts.SaveData;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace GuildMasterIsekai
{
    public static class Game
    {
        private static Database _database;
        private static UIntGenerator _id;
        private static SaveManager _save;

        public static Database Database => Load(ref _database);
        public static UIntGenerator Id { get; } = new();
        public static SaveManager Save { get; } = new("Save", "Default", new SaveFileParser());

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

        public static Sprite LoadSprite(string path) => Addressables.LoadAssetAsync<Sprite>($"Sprites/{path}").WaitForCompletion();
    }

    public static class ObjectDB<T>
    {
        private static readonly Dictionary<uint, T> _items = new();
        public static T Get(uint id) => _items.GetValueOrDefault(id);
        public static void Add(uint id, T item) => _items.Add(id, item);
        public static bool Remove(uint id) => _items.Remove(id);
    }
}