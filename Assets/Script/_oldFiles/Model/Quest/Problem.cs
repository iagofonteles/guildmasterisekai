using System.IO;
using Drafts.SaveData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GuildMasterIsekai {

	[Serializable]
	public class Problem : IDisplay, IGuid, IBinarySave {
		[SerializeField] eStat mainStat;
		[SerializeField] string guid;
		[SerializeField] string displayName;
		[SerializeField] string description;
		[SerializeField] string icon;
		[SerializeField] string rank;
		[SerializeField] List<Quest>[] outcomes = new List<Quest>[0]; //TODO fix serialization
		public event Action OnChanged;

		Sprite _icon;
		RankSo _rank;

		public string Guid => guid;
		public string DisplayName => displayName;
		public string Description => description;
		public Sprite Icon => _icon ??= Resources.Load<Sprite>("Problem Icon/" + icon);
		public RankSo Rank => _rank ??= Game.Database.Find<RankSo>(rank);

		public Problem(RankSo rank, eStat mainStat, string displayName, string description, string icon) {
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

		public Problem() { }

		public void Save(BinaryWriter writer) {
			writer.Write((int)mainStat);
			writer.Write(guid != null);
			if (guid != null) writer.Write(guid);
			writer.Write(displayName != null);
			if (displayName != null) writer.Write(displayName);
			writer.Write(description != null);
			if (description != null) writer.Write(description);
			writer.Write(icon != null);
			if (icon != null) writer.Write(icon);
			writer.Write(rank != null);
			if (rank != null) writer.Write(rank);
			writer.WriteCollection(static (w, quests) => w.Write(quests), outcomes);
		}

		public void Load(BinaryReader reader) {
			mainStat = (eStat)reader.ReadInt32();
			guid = reader.ReadBoolean() ? reader.ReadString() : null;
			displayName = reader.ReadBoolean() ? reader.ReadString() : null;
			description = reader.ReadBoolean() ? reader.ReadString() : null;
			icon = reader.ReadBoolean() ? reader.ReadString() : null;
			rank = reader.ReadBoolean() ? reader.ReadString() : null;
			outcomes = reader.ReadArray(static r => new List<Quest>(r.ReadCollection<Quest>()));
			_rank = null;
			_icon = null;
		}
	}
}
