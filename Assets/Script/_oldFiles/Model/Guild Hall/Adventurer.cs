using System.IO;
using Drafts.SaveData;
using Drafts;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace GuildMasterIsekai {

	[Serializable]
	public class Adventurer : IDisplay, IGuid, IVisitor, IBinarySave {
		[SerializeField, ReadOnly] string guid;
		[SerializeField] string displayName;
		[SerializeField, Rank] string rank;
		[SerializeField, Rank] string portrait;
		public Stats stats;
		public Assignement assignement;
		public event Action OnChanged;

		RankSo _rank;
		Sprite _portrait;

		public Adventurer(RankSo rank, string nickname, string portrait, Stats stats) {
			guid = System.Guid.NewGuid().ToString();
			displayName = nickname;
			this.portrait = portrait;
			this.rank = rank.name;
			this.stats = stats;
		}

		public RankSo Rank => _rank ??= Game.Database.Find<RankSo>(rank);

		public string Guid => guid;
		public string Name => nameof(Adventurer);
		public string DisplayName { get => displayName; set { displayName = value; OnChanged?.Invoke(); } }
		public string Description => "...";
		public Sprite Icon => _portrait ??= Resources.Load<Sprite>($"Portrait/{portrait}");

		public IVisitorAI GetAI(NavMeshAgent agent, HallSpots spots)
			=> new FreelancerVisitorAI(this, agent, spots);

		public Adventurer() { }

		public void Save(BinaryWriter writer) {
			writer.Write(guid != null);
			if (guid != null) writer.Write(guid);
			writer.Write(displayName != null);
			if (displayName != null) writer.Write(displayName);
			writer.Write(rank != null);
			if (rank != null) writer.Write(rank);
			writer.Write(portrait != null);
			if (portrait != null) writer.Write(portrait);
			writer.Write(stats != null);
			stats?.Save(writer);
			writer.Write(assignement != null);
			assignement?.Save(writer);
		}

		public void Load(BinaryReader reader) {
			guid = reader.ReadBoolean() ? reader.ReadString() : null;
			displayName = reader.ReadBoolean() ? reader.ReadString() : null;
			rank = reader.ReadBoolean() ? reader.ReadString() : null;
			portrait = reader.ReadBoolean() ? reader.ReadString() : null;
			stats = reader.ReadBoolean() ? new Stats() : null;
			stats?.Load(reader);
			assignement = reader.ReadBoolean() ? new Assignement() : null;
			assignement?.Load(reader);
			_rank = null;
			_portrait = null;
		}
	}

}
