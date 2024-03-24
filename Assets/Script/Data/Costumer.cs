using Drafts;
using System;
using UnityEngine;

namespace GuildMasterIsekai {

	[Serializable]
	public class Costumer : IDisplay, IGuid, IVisitor {
		[SerializeField, ReadOnly] string guid;
		[SerializeField] string displayName;
		[SerializeField, Rank] string rank;
		[SerializeField, Rank] string portrait;
		public event Action OnChanged;

		Rank _rank;
		Sprite _portrait;

		public Costumer(Rank rank, string nickname, string portrait) {
			guid = System.Guid.NewGuid().ToString();
			displayName = nickname;
			this.portrait = portrait;
			this.rank = rank.name;
			_rank = rank;
		}

		public Rank Rank => _rank ??= Game.Database.Find<Rank>(rank);

		public string Guid => guid;
		public string Name => nameof(Adventurer);
		public string DisplayName => displayName;
		public string Description => "...";
		public Sprite Icon => _portrait ??= Resources.Load<Sprite>($"Portrait/{portrait}");
	}
}
