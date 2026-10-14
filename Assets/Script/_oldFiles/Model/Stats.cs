using System.IO;
using Drafts.SaveData;
using System;
using TMPro;
using UnityEngine;

namespace GuildMasterIsekai {

	[Serializable]
	public class Stats : IBinarySave {
		[SerializeField] int power;
		[SerializeField] int knoledge;
		[SerializeField] int vigor;

		public int Knoledge { get => knoledge; set { knoledge = value; OnChanged?.Invoke(); } }
		public int Vigor { get => vigor; set { vigor = value; OnChanged?.Invoke(); } }
		public int Power { get => power; set { power = value; OnChanged?.Invoke(); } }

		public event Action OnChanged;

		public void Save(BinaryWriter writer) {
			writer.Write(power);
			writer.Write(knoledge);
			writer.Write(vigor);
		}

		public void Load(BinaryReader reader) {
			power = reader.ReadInt32();
			knoledge = reader.ReadInt32();
			vigor = reader.ReadInt32();
		}
	}
}
