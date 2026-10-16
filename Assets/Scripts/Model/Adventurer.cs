using System;
using System.IO;
using Drafts;
using Drafts.SaveData;
using GuildMasterIsekai.Database;
using GuildMasterIsekai.Systems;
using GuildMasterIsekai.Visitor;
using GuildMasterIsekai.Visitor.Spot;
using UnityEngine;
using UnityEngine.AI;

namespace GuildMasterIsekai.Model
{
    [Serializable]
    public class Adventurer : IDisplay, IVisitor, IBinarySave
    {
        [SerializeField, ReadOnly] private uint id;
        [SerializeField] private string portrait;
        [SerializeField] private Watcher<string> displayName = new();
        [SerializeField] private Watcher<Rank> rank = new();
        [SerializeField] private Stats stats;
        public Assignment assignment;

        public Adventurer()
        {
            stats = new();
        }

        public Adventurer(
            string portrait, string nickname,
            Rank rank, Stats stats)
        {
            id = Game.Id.Generate();
            displayName.Value = nickname;
            this.portrait = portrait;
            this.rank.Value = rank;
            this.stats = stats;
        }

        public Watcher<Rank> Rank => rank;
        public string Name => nameof(Adventurer);
        public Stats Stats => stats;

        public void Save(BinaryWriter writer)
        {
            writer.Write(id);
            writer.Write(portrait ?? "");
            writer.Write(displayName.Value ?? "");
            writer.Write((int)rank.Value);
            writer.WriteSave(assignment);

            stats.Save(writer);
        }

        public void Load(BinaryReader reader)
        {
            id = reader.ReadUInt32();
            portrait = reader.ReadString();
            displayName.Value = reader.ReadString();
            rank.Value = (Rank)reader.ReadInt32();
            assignment = reader.ReadSave<Assignment>();

            stats?.Load(reader);
        }

        public event Action OnChanged;

        public uint Id => id;
        public string DisplayName => displayName.Value;
        public string Description => "...";
        public Sprite Icon => Game.LoadSprite(portrait);

        public IVisitorAI GetAI(NavMeshAgent agent, HallSpots spots)
            => new FreelancerVisitorAI(this, agent, spots);
    }
}