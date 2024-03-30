using System;
using UnityEngine;

namespace GuildMasterIsekai {
	[Serializable]
	public class Quest : IDisplay, IGuid {
		[SerializeField] string guid;
		[SerializeField] string displayName;
		[SerializeField] string description;
		[SerializeField] string rank;
		[SerializeField] string icon;
		[SerializeField] int fee;
		[SerializeField] Stats requisite;
		[SerializeField] Reward reward;

		Rank _rank;
		Sprite _icon;

		public event Action OnChanged;

		public string Guid => guid;
		public string DisplayName => displayName;
		public string Description => description;
		public Rank Rank => _rank ??= Game.Database.Find<Rank>(rank);
		public Sprite Icon => _icon ??= Resources.Load<Sprite>("Photos/" + icon);
		public int Fee => fee;
		public Stats Requisite => requisite;
		public Reward Reward => reward;

		public Quest() {
			guid = System.Guid.NewGuid().ToString();
		}

		public Quest(Rank rank, string displayName, string description, int fee, Stats requisite, Reward reward) {
			guid = System.Guid.NewGuid().ToString();
			this.rank = rank.name;
			this.displayName = displayName;
			this.description = description;
			this.reward = reward;
			this.fee = fee;
			this.requisite = requisite;
		}
	}
}
