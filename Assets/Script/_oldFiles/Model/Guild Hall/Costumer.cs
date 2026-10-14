using System.IO;
using Drafts.SaveData;
using Drafts;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace GuildMasterIsekai {

	[Serializable]
	public class Costumer : IDisplay, IGuid, IVisitor, IBinarySave {
		[SerializeField, ReadOnly] string guid;
		[SerializeField] string displayName;
		[SerializeField, Rank] string rank;
		[SerializeField] eStat stat;
		[SerializeField] string portrait;
		public event Action OnChanged;

		RankSo _rank;
		Sprite _portrait;

		public Costumer(RankSo rank, string nickname, string portrait, eStat stat) {
			guid = System.Guid.NewGuid().ToString();
			displayName = nickname;
			this.portrait = portrait;
			this.rank = rank.name;
			this.stat = stat;
			_rank = rank;
		}

		public RankSo Rank => _rank ??= Game.Database.Find<RankSo>(rank);
		public eStat Stat => stat;

		public string Guid => guid;
		public string Name => nameof(Adventurer);
		public string DisplayName => displayName;
		public string Description => "...";
		public Sprite Icon => _portrait ??= Resources.Load<Sprite>($"Portrait/{portrait}");

		public IVisitorAI GetAI(NavMeshAgent agent, HallSpots spots)
			=> new CostumerVisitorAI(this, agent, spots);

		public Costumer() { }

		public void Save(BinaryWriter writer) {
			writer.Write(guid != null);
			if (guid != null) writer.Write(guid);
			writer.Write(displayName != null);
			if (displayName != null) writer.Write(displayName);
			writer.Write(rank != null);
			if (rank != null) writer.Write(rank);
			writer.Write((int)stat);
			writer.Write(portrait != null);
			if (portrait != null) writer.Write(portrait);
		}

		public void Load(BinaryReader reader) {
			guid = reader.ReadBoolean() ? reader.ReadString() : null;
			displayName = reader.ReadBoolean() ? reader.ReadString() : null;
			rank = reader.ReadBoolean() ? reader.ReadString() : null;
			stat = (eStat)reader.ReadInt32();
			portrait = reader.ReadBoolean() ? reader.ReadString() : null;
			_rank = null;
			_portrait = null;
		}
	}
}
