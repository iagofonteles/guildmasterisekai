using System;
using System.Collections.Generic;
using UnityEngine;

namespace GuildMasterIsekai {

	[Serializable]
	public class Problem : IDisplay, IGuid {
		[SerializeField] eStat mainStat;
		[SerializeField] string guid;
		[SerializeField] string displayName;
		[SerializeField] string description;
		[SerializeField] string icon;
		[SerializeField] string rank;
		[SerializeField] List<Quest>[] outcomes = new List<Quest>[0]; //TODO fix serialization
		public event Action OnChanged;

		Sprite _icon;
		Rank _rank;

		public string Guid => guid;
		public string DisplayName => displayName;
		public string Description => description;
		public Sprite Icon => _icon ??= Resources.Load<Sprite>("Problem Icon/" + icon);
		public Rank Rank => _rank ??= Game.Database.Find<Rank>(rank);

		public Problem(Rank rank, eStat mainStat, string displayName, string description, string icon) {
			guid = System.Guid.NewGuid().ToString();
			this.rank = rank.name;
			this.mainStat = mainStat;
			this.displayName = displayName;
			this.description = description;
			this.icon = icon;
		}

		public IReadOnlyList<Quest>[] Outcomes
			=> outcomes.Length == 0 ? outcomes = GetOutcomes() : outcomes;

		private List<Quest>[] GetOutcomes() {
			var generator = Game.Save.Get<Guild>().Generator;

			var easy = new List<Quest>() {
				generator.NewQuest(Rank, mainStat),
				generator.NewQuest(Rank, mainStat),
				generator.NewQuest(Rank, mainStat),
			};
			var med = new List<Quest>() {
				generator.NewQuest(Rank, mainStat),
				generator.NewQuest(Rank, mainStat),
			};
			var hard = new List<Quest>() {
				generator.NewQuest(Rank, mainStat),
			};
			return new List<Quest>[] { easy, med, hard };
		}

	}
}
