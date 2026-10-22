using System;
using System.IO;
using Drafts;
using Drafts.SaveData;
using GuildMasterIsekai.Database;
using UnityEngine;

namespace GuildMasterIsekai.Model
{
    [Serializable]
    public class Guild : IBinarySave
    {
        [SerializeField] private Rank rank = Rank.F;
        [SerializeField] private Observable<int> gold = new();
        [SerializeField] private ObservableList<Adventurer> adventurers = new();
        private Generator _generator;

        public Rank Rank { get => rank; set => rank = value; }
        public Observable<int> Gold => gold;
        public ObservableList<Adventurer> Adventurers => adventurers;
        public Generator Generator => _generator ??= new Generator(this);

        public void Save(BinaryWriter writer)
        {
            writer.Write((int)rank);
            writer.Write(gold.Value);
            writer.WriteSaveCollection(adventurers);
        }

        public void Load(BinaryReader reader)
        {
            rank = (Rank)reader.ReadInt32();
            gold.Value = reader.ReadInt32();
            adventurers.AddRange(reader.ReadSaveCollection<Adventurer>());
        }
    }
}