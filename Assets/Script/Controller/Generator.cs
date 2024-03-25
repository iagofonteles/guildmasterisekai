using Drafts;
using UnityEngine;

namespace GuildMasterIsekai {
	public class Generator {

		private Guild guild;

		public Generator(Guild guild) {
			this.guild = guild;
		}

		public Adventurer NewAdventurer() {
			return new Adventurer(guild.Rank, "Novo Aventureiro", "Default", Stats(guild.Rank));
		}

		public Costumer NewCostumer() {
			return new Costumer(guild.Rank, "Novo Aventureiro", "Default", (eStat)Random.Range(0, 2));
		}

		public Stats Stats(Rank rank) {
			return new Stats {
				Power = Random.Range(1, 2),
				Knoledge = Random.Range(1, 2),
				Resistance = Random.Range(1, 2),
			};
		}

		public Problem NewProblem(Costumer costumer) {
			return new Problem(costumer.Rank, costumer.Stat, "Nova Missão", "---", "Default");
		}

		public Quest NewQuest(Rank rank, eStat mainStat) {
			var reward = NewReward(rank, mainStat, 1);
			return new Quest(rank, "Nova Missão", "Default", 10, reward);
		}

		public Reward NewReward(Rank rank, eStat stat, int lootAmount) {
			var loot = Game.Database.All<Loot>().Random(lootAmount);
			return new Reward(Random.Range(100, 200), loot);
		}
	}

}
