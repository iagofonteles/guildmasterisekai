using Drafts.SaveData;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace GuildMasterIsekai.System
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
}