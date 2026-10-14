using System.IO;
using Drafts.SaveData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GuildMasterIsekai
{
    [Serializable]
    public class Reward : IBinarySave
    {
        [SerializeField] int gold;
        [SerializeField] List<LootSo> items = new();
        public int Gold => gold;
        public IReadOnlyList<LootSo> Items => items;

        public Reward() { }
        public Reward(int gold, IEnumerable<LootSo> items)
        {
            this.gold = gold;
            this.items = new(items);
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(gold);
            writer.WriteSaveCollection(items);
        }

        public void Load(BinaryReader reader)
        {
            gold = reader.ReadInt32();
            items = new(reader.ReadCollection<LootSo>());
        }
    }
}