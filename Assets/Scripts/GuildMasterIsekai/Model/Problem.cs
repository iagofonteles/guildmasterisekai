using System;
using System.Collections.Generic;
using System.IO;
using Drafts;
using Drafts.SaveData;
using GuildMasterIsekai.Database;
using GuildMasterIsekai.Systems;
using UnityEngine;

namespace GuildMasterIsekai.Model
{
    [Serializable]
    public class Problem : IDisplay, IBinarySave
    {
        [SerializeField, ReadOnly] private uint id;
        [SerializeField] private Stat mainStat;
        [SerializeField] private string displayName;
        [SerializeField] private string description;
        [SerializeField] private string icon;
        [SerializeField] private Rank rank;
        private List<Quest>[] outcomes; //TODO fix serialization

        public Problem(Rank rank, Stat mainStat, string displayName, string description, string icon)
        {
            id = Game.Id.Generate();
            this.rank = rank;
            this.mainStat = mainStat;
            this.displayName = displayName;
            this.description = description;
            this.icon = icon;
        }

        public Problem() { }
        public Rank Rank => rank;

        public IReadOnlyList<Quest>[] Outcomes => outcomes ??= GetOutcomes();

        public void Save(BinaryWriter writer)
        {
            writer.Write(id);
            writer.Write((int)mainStat);
            writer.Write(displayName ?? "");
            writer.Write(description ?? "");
            writer.Write(icon ?? "");
            writer.Write((int)rank);
            //TODO writer.WriteSaveCollection(outcomes);
        }

        public void Load(BinaryReader reader)
        {
            id = reader.ReadUInt32();
            mainStat = (Stat)reader.ReadInt32();
            displayName = reader.ReadString();
            description = reader.ReadString();
            icon = reader.ReadString();
            rank = (Rank)reader.ReadInt32();
            //TODO outcomes = reader.ReadArray(Quest);
        }

        public event Action OnChanged;

        public uint Id => id;
        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Icon => Game.LoadSprite(icon);

        private List<Quest>[] GetOutcomes()
        {
            var generator = Game.SaveMngr.Get<Guild>().Generator;

            var easy = new List<Quest>
            {
                generator.NewQuest(Rank, mainStat),
                generator.NewQuest(Rank, mainStat),
                generator.NewQuest(Rank, mainStat)
            };
            var med = new List<Quest>
            {
                generator.NewQuest(Rank, mainStat),
                generator.NewQuest(Rank, mainStat)
            };
            var hard = new List<Quest>
            {
                generator.NewQuest(Rank, mainStat)
            };
            return new[] { easy, med, hard };
        }
    }
}