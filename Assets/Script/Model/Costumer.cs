using System;
using System.IO;
using Drafts;
using Drafts.SaveData;
using GuildMasterIsekai.Database;
using GuildMasterIsekai.System;
using GuildMasterIsekai.Visitor;
using GuildMasterIsekai.Visitor.Spot;
using UnityEngine;
using UnityEngine.AI;

namespace GuildMasterIsekai.Model
{
    [Serializable]
    public class Costumer : IDisplay, IVisitor, IBinarySave
    {
        [SerializeField, ReadOnly] private uint id;
        [SerializeField] private string displayName;
        [SerializeField] private string portrait;
        [SerializeField] private Rank rank;
        [SerializeField] private Stat stat;
        public event Action OnChanged;

        public uint Id => id;
        public string Name => nameof(Adventurer);
        public string DisplayName => displayName;
        public string Description => "...";
        public Sprite Icon => Game.LoadSprite(portrait);
        public Rank Rank => rank;
        public Stat Stat => stat;

        public Costumer() { }
        public Costumer(Rank rank, string nickname, string portrait, Stat stat)
        {
            id = Game.Id.Generate();
            displayName = nickname;
            this.portrait = portrait;
            this.rank = rank;
            this.stat = stat;
        }

        public IVisitorAI GetAI(NavMeshAgent agent, HallSpots spots)
            => new CostumerVisitorAI(this, agent, spots);

        public void Save(BinaryWriter writer)
        {
            writer.Write(id);
            writer.Write(displayName ?? "");
            writer.Write(portrait ?? "");
            writer.Write((int)rank);
            writer.Write((int)stat);
        }

        public void Load(BinaryReader reader)
        {
            id = reader.ReadUInt32();
            displayName = reader.ReadString();
            portrait = reader.ReadString();
            rank = (Rank)reader.ReadInt32();
            stat = (Stat)reader.ReadInt32();
        }
    }
}