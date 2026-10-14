using System.IO;
using Drafts.SaveData;
using System;
using UnityEngine;

namespace GuildMasterIsekai {
	[Serializable]
	public class Quest : IDisplay, IGuid, IBinarySave {
		[SerializeField] private uint guid;
		[SerializeField] private string displayName;
		[SerializeField] private string description;
		[SerializeField] private int rank;
		[SerializeField] private string icon;
		[SerializeField] private int fee;
		[SerializeField] private Stats requisite;
		[SerializeField] private Reward reward;

		private RankSo _rank;
		private Sprite _icon;

		public event Action OnChanged;

		public string Guid => guid.ToString();
		public string DisplayName => displayName;
		public string Description => description;
		public RankSo Rank => _rank ??= Game.Database.Find<RankSo>(rank);
		public Sprite Icon => _icon ??= Resources.Load<Sprite>("Photos/" + icon);
		public int Fee => fee;
		public Stats Requisite => requisite;
		public Reward Reward => reward;

		public Quest() {
			guid = System.Guid.NewGuid().ToString();
		}

		public Quest(RankSo rank, string displayName, string description, int fee, Stats requisite, Reward reward) {
			guid = System.Guid.NewGuid().ToString();
			this.rank = rank.name;
			this.displayName = displayName;
			this.description = description;
			this.reward = reward;
			this.fee = fee;
			this.requisite = requisite;
		}

		public void Save(BinaryWriter writer) {
			writer.Write(guid);
			writer.Write(displayName != null);
			if (displayName != null) writer.Write(displayName);
			writer.Write(description != null);
			if (description != null) writer.Write(description);
			writer.Write(rank);
			writer.Write(icon != null);
			if (icon != null) writer.Write(icon);
			writer.Write(fee);
			writer.Write(requisite != null);
			requisite?.Save(writer);
			writer.Write(reward != null);
			reward?.Save(writer);
		}

		public void Load(BinaryReader reader) {
			guid = reader.ReadUInt32();
			displayName = reader.ReadBoolean() ? reader.ReadString() : null;
			description = reader.ReadBoolean() ? reader.ReadString() : null;
			rank = reader.ReadInt32();
			icon = reader.ReadBoolean() ? reader.ReadString() : null;
			fee = reader.ReadInt32();
			requisite = reader.ReadBoolean() ? new Stats() : null;
			requisite?.Load(reader);
			reward = reader.ReadBoolean() ? new Reward() : null;
			reward?.Load(reader);
			_rank = null;
			_icon = null;
		}
	}
}
