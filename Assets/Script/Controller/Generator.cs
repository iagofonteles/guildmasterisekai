using UnityEngine;

namespace GuildMasterIsekai {
	public class Generator {

		private Guild guild;

		public Generator(Guild guild) {
			this.guild = guild;
		}

		public Adventurer Adventurer() {
			var rank = Game.Database.Find<Rank>("E");
			return new Adventurer(rank, "Novo Aventureiro", "Default", Stats(rank));
		}

		public Stats Stats(Rank rank) {
			return new Stats {
				Power = Random.Range(1, 2),
				Knoledge = Random.Range(1, 2),
				Resistance = Random.Range(1, 2),
			};
		}
	}
}
