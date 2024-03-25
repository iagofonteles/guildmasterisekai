using Drafts;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace GuildMasterIsekai {

	[Serializable]
	public class Costumer : IDisplay, IGuid, IVisitor {
		[SerializeField, ReadOnly] string guid;
		[SerializeField] string displayName;
		[SerializeField, Rank] string rank;
		[SerializeField] eStat stat;
		[SerializeField] string portrait;
		public event Action OnChanged;

		Rank _rank;
		Sprite _portrait;

		public Costumer(Rank rank, string nickname, string portrait, eStat stat) {
			guid = System.Guid.NewGuid().ToString();
			displayName = nickname;
			this.portrait = portrait;
			this.rank = rank.name;
			this.stat = stat;
			_rank = rank;
		}

		public Rank Rank => _rank ??= Game.Database.Find<Rank>(rank);
		public eStat Stat => stat;

		public string Guid => guid;
		public string Name => nameof(Adventurer);
		public string DisplayName => displayName;
		public string Description => "...";
		public Sprite Icon => _portrait ??= Resources.Load<Sprite>($"Portrait/{portrait}");

		public IVisitorAI GetAI(NavMeshAgent agent, HallSpots spots)
			=> new CostumerVisitorAI(this, agent, spots);
	}
}
