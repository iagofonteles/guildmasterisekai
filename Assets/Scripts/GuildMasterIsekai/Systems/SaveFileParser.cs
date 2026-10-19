using System;
using System.IO;
using Drafts.SaveData;
using UnityEngine;

namespace GuildMasterIsekai.Systems
{
    public class SaveFileParser : FileParserBase
    {
        protected override string Extension => ".binsav";

        public override void Save(string key, in object data)
        {
            if (data is not IBinarySave sd) throw new Exception($"{data?.GetType().Name} is not ISaveData");
            AssurePath(key);
            using var file = File.Create(key);
            using var writer = new BinaryWriter(file);
            sd.Save(writer);
        }

        public override object Load(string key, Type type) => Load(key, type, null);

        public object Load(string key, Type type, params object[] args)
        {
            var obj = Activator.CreateInstance(type, args);
            if (obj is not IBinarySave sd) throw new Exception($"{type.Name} is not ISaveData");
            if (!File.Exists(key)) return obj;

            try
            {
                using var file = File.OpenRead(key);
                using var reader = new BinaryReader(file);
                sd.Load(reader);
                return obj;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading {key} {type.Name}");
                Debug.LogException(e);
                return Activator.CreateInstance(type);
            }
        }
    }
}