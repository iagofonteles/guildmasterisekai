using GuildMasterIsekai.Database;
using GuildMasterIsekai.Model;
using GuildMasterIsekai.System;
using UnityEngine;

namespace GuildMasterIsekai
{
    public class Generator
    {
        private static int RandomNo => Random.Range(10, 99);

        private Guild guild;

        public Generator(Guild guild)
        {
            this.guild = guild;
        }

        public Adventurer NewAdventurer() =>
            new("Default", "Aventureiro " + RandomNo,
                guild.Rank, Stats(guild.Rank));

        public Costumer NewCostumer() =>
            new(guild.Rank, "Consumidor " + RandomNo,
                "Default", (Stat)Random.Range(0, 2));

        private Stats Stats(Rank rank)
        {
            return new Stats
            {
                Power = { Value = Random.Range(1, 2) },
                Knowledge = { Value = Random.Range(1, 2) },
                Vigor = { Value = Random.Range(1, 2) },
            };
        }

        public Problem NewProblem(Costumer costumer)
        {
            return new Problem(costumer.Rank, costumer.Stat, "Problema " + RandomNo, "---", "Default");
        }

        public Quest NewQuest(Rank rank, Stat mainStat)
        {
            var reward = NewReward(rank, mainStat, 1);
            var stats = new Stats
            {
                Power = { Value = Random.Range(1, 4) },
                Knowledge = { Value = Random.Range(1, 4) },
                Vigor = { Value = Random.Range(1, 4) },
            };
            
            var desc = "Missao description";
            return new Quest("Default", "Missão " + RandomNo, desc, rank, 10, stats, reward);
        }

        public Reward NewReward(Rank rank, Stat stat, int lootAmount)
        {
            var loot = Game.Database.GetAll<LootSo>().Random(lootAmount);
            return new Reward(Random.Range(100, 200), loot);
        }
    }
}