using Drafts;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace GuildMasterIsekai {

	[Serializable]
	public class Adventurer : IDisplay, IGuid, IVisitor {
		[SerializeField, ReadOnly] string guid;
		[SerializeField] string displayName;
		[SerializeField, Rank] string rank;
		[SerializeField, Rank] string portrait;
		public Stats stats;
		public Assignement assignement;
		public event Action OnChanged;

		Rank _rank;
		Sprite _portrait;

		public Adventurer(Rank rank, string nickname, string portrait, Stats stats) {
			guid = System.Guid.NewGuid().ToString();
			displayName = nickname;
			this.portrait = portrait;
			this.rank = rank.name;
			this.stats = stats;
		}

		public Rank Rank => _rank ??= Game.Database.Find<Rank>(rank);

		public string Guid => guid;
		public string Name => nameof(Adventurer);
		public string DisplayName { get => displayName; set { displayName = value; OnChanged?.Invoke(); } }
		public string Description => "...";
		public Sprite Icon => _portrait ??= Resources.Load<Sprite>($"Portrait/{portrait}");

		public IVisitorAI GetAI(NavMeshAgent agent, HallSpots spots)
			=> new FreelancerVisitorAI(this, agent, spots);
	}

}
