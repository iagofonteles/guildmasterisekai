using System;
using TMPro;
using UnityEngine;

namespace GuildMasterIsekai {

	[Serializable]
	public class Stats {
		[SerializeField] int power;
		[SerializeField] int knoledge;
		[SerializeField] int vigor;

		public int Knoledge { get => knoledge; set { knoledge = value; OnChanged?.Invoke(); } }
		public int Vigor { get => vigor; set { vigor = value; OnChanged?.Invoke(); } }
		public int Power { get => power; set { power = value; OnChanged?.Invoke(); } }

		public event Action OnChanged;
	}
}
