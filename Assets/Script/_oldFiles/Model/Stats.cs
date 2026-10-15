using System.IO;
using Drafts.SaveData;
using System;
using UnityEngine;

namespace GuildMasterIsekai {

	[Serializable]
	public class Stats : IBinarySave {
		[SerializeField] private int power;
		[SerializeField] private int knowledge;
		[SerializeField] private int vigor;

		public int Knowledge { get => knowledge; set { knowledge = value; OnChanged?.Invoke(); } }
		public int Vigor { get => vigor; set { vigor = value; OnChanged?.Invoke(); } }
		public int Power { get => power; set { power = value; OnChanged?.Invoke(); } }

		public event Action OnChanged;

		public void Save(BinaryWriter writer) {
			writer.Write(power);
			writer.Write(knowledge);
			writer.Write(vigor);
		}

		public void Load(BinaryReader reader) {
			power = reader.ReadInt32();
			knowledge = reader.ReadInt32();
			vigor = reader.ReadInt32();
		}
	}
}
