using System.IO;
using Drafts.SaveData;
using System;
using Drafts;
using UnityEngine;

namespace GuildMasterIsekai
{
    [Serializable]
    public class Quest : IDisplay, IBinarySave
    {
        [SerializeField, ReadOnly] private uint id;
        [SerializeField] private string icon;
        [SerializeField] private string displayName;
        [SerializeField] private string description;
        [SerializeField] private Rank rank;
        [SerializeField] private int fee;
        [SerializeField] private Stats requisite;
        [SerializeField] private Reward reward;

        public event Action OnChanged;

        public uint Id => id;
        public Sprite Icon => Game.LoadSprite(icon);
        public string DisplayName => displayName;
        public string Description => description;
        public Rank Rank => rank;
        public int Fee => fee;
        public IStats Requisite => requisite;
        public Reward Reward => reward;

        public Quest()
        {
            requisite = new();
            reward = new();
        }

        public Quest(
            string icon, string displayName, string description,
            Rank rank, int fee, Stats requisite, Reward reward)
        {
            id = Game.Id.Generate();
            this.icon = icon;
            this.displayName = displayName;
            this.description = description;
            this.rank = rank;
            this.fee = fee;
            this.requisite = requisite;
            this.reward = reward;
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(id);
            writer.Write(icon ?? "");
            writer.Write(displayName ?? "");
            writer.Write(description ?? "");
            writer.Write((int)rank);
            writer.Write(fee);
            requisite.Save(writer);
            reward.Save(writer);
        }

        public void Load(BinaryReader reader)
        {
            id = reader.ReadUInt32();
            icon = reader.ReadString();
            displayName = reader.ReadString();
            description = reader.ReadString();
            rank = (Rank)reader.ReadInt32();
            fee = reader.ReadInt32();
            requisite?.Load(reader);
            reward?.Load(reader);
        }
    }
}