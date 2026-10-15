using Drafts;
using UnityEngine;

namespace GuildMasterIsekai {
	public class Generator {

		int RandomNo => Random.Range(10, 99);

		private Guild guild;

		public Generator(Guild guild) {
			this.guild = guild;
		}

		public Adventurer NewAdventurer() {
			return new Adventurer(guild.Rank, "Aventureiro " + RandomNo,
				"Default", Stats(guild.Rank));
		}

		public Costumer NewCostumer() {
			return new Costumer(guild.Rank, "Consumidor " + RandomNo,
				"Default", (eStat)Random.Range(0, 2));
		}

		public Stats Stats(RankSo rank) {
			return new Stats {
				Power = Random.Range(1, 2),
				Knowledge = Random.Range(1, 2),
				Vigor = Random.Range(1, 2),
			};
		}

		public Problem NewProblem(Costumer costumer) {
			return new Problem(costumer.Rank, costumer.Stat, "Problema " + RandomNo, "---", "Default");
		}

		public Quest NewQuest(RankSo rank, eStat mainStat) {
			var reward = NewReward(rank, mainStat, 1);
			var stats = new Stats {
				Power = Random.Range(1, 4),
				Knowledge = Random.Range(1, 4),
				Vigor = Random.Range(1, 4),
			};

			return new Quest(rank, "Missão " + RandomNo, "Default", 10, stats, reward);
		}

		public Reward NewReward(RankSo rank, eStat stat, int lootAmount) {
			var loot = Game.Database.GetAll<LootSo>().Random(lootAmount);
			return new Reward(Random.Range(100, 200), loot);
		}
	}

}
