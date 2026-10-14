using System;
using System.IO;
using Drafts.SaveData;
using UnityEngine;

namespace GuildMasterIsekai
{
    public interface ISaveData
    {
        void BeforeSave();
        void AfterLoad();
    }

    public class SaveFileParser : FileParserBase
    {
        protected override string Extension => ".json";
        private bool Beautify { get; }

        public SaveFileParser(bool beautify = true) => Beautify = beautify;

        public override void Save(string key, in object data)
        {
            if (data is ISaveData sd) sd.BeforeSave();
            var json = JsonUtility.ToJson(data, Beautify);
            AssurePath(key);
            File.WriteAllText(key, json);
        }

        public override object Load(string key, Type type)
        {
            var data = File.Exists(key)
                ? JsonUtility.FromJson(File.ReadAllText(key), type)
                : Activator.CreateInstance(type);
            if (data is ISaveData sd) sd.AfterLoad();
            return data;
        }
    }
}