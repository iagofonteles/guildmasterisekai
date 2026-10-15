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
        [SerializeField] private int gold;
        [SerializeField] private List<LootSo> items = new();
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
            writer.WriteItemCollection(items);
        }

        public void Load(BinaryReader reader)
        {
            gold = reader.ReadInt32();
            items = new(reader.ReadItemCollection<LootSo>());
        }
    }
}