using System.IO;
using Drafts.SaveData;
using Drafts;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace GuildMasterIsekai
{
    [Serializable]
    public class Adventurer : IDisplay, IVisitor, IBinarySave
    {
        [SerializeField, ReadOnly] private uint id;
        [SerializeField] private string portrait;
        [SerializeField] private Observable<string> displayName;
        [SerializeField] private Observable<Rank> rank;
        public Stats stats;
        public Assignment assignment;
        public event Action OnChanged;

        public Adventurer()
        {
            stats = new();
        }
        
        public Adventurer(Rank rank, string nickname, string portrait, Stats stats)
        {
            id = Game.Id.Generate();
            displayName = nickname;
            this.portrait = portrait;
            this.rank = rank;
            this.stats = stats;
        }

        public RankSo Rank => _rank ??= Game.Database.Find<RankSo>(rank);

        public uint Id => id;
        public string Name => nameof(Adventurer);

        public string DisplayName
        {
            get => displayName;
            set {
                displayName = value;
                OnChanged?.Invoke();
            }
        }

        public string Description => "...";
        public Sprite Icon => _portrait ??= Resources.Load<Sprite>($"Portrait/{portrait}");

        public IVisitorAI GetAI(NavMeshAgent agent, HallSpots spots)
            => new FreelancerVisitorAI(this, agent, spots);

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
    }
}