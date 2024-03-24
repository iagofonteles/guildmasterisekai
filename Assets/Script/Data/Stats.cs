using System;
namespace GuildMasterIsekai {

	[Serializable]
	public class Stats {
		private int power;
		private int resistance;
		private int knoledge;

		public int Knoledge { get => knoledge; set { knoledge = value; OnChanged?.Invoke(); } }
		public int Resistance { get => resistance; set { resistance = value; OnChanged?.Invoke(); } }
		public int Power { get => power; set { power = value; OnChanged?.Invoke(); } }

		public event Action OnChanged;
	}
}
