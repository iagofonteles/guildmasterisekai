using System;
using System.IO;
using Drafts.SaveData;
using GuildMasterIsekai.System;
using UnityEngine;

namespace GuildMasterIsekai.Model
{
    public interface IStats
    {
        int Knowledge { get; }
        int Vigor { get; }
        int Power { get; }
    }

    [Serializable]
    public class Stats : IStats, IBinarySave
    {
        [SerializeField] private Watcher<int> power = new();
        [SerializeField] private Watcher<int> knowledge = new();
        [SerializeField] private Watcher<int> vigor = new();

        public Watcher<int> Power => power;
        public Watcher<int> Knowledge => knowledge;
        public Watcher<int> Vigor => vigor;

        public void Save(BinaryWriter writer)
        {
            writer.Write(power.Value);
            writer.Write(knowledge.Value);
            writer.Write(vigor.Value);
        }

        public void Load(BinaryReader reader)
        {
            power.Value = reader.ReadInt32();
            knowledge.Value = reader.ReadInt32();
            vigor.Value = reader.ReadInt32();
        }

        int IStats.Knowledge => knowledge.Value;
        int IStats.Vigor => vigor.Value;
        int IStats.Power => power.Value;
    }
}