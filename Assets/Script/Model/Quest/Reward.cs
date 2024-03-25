using System;
using System.Collections.Generic;
using UnityEngine;

namespace GuildMasterIsekai {
	[Serializable]
	public class Reward {
		[SerializeField] int gold;
		[SerializeField] List<Loot> itens = new();
		public int Gold => gold;
		public IReadOnlyList<Loot> Itens => itens;

		public Reward(int gold, IEnumerable<Loot> itens) {
			this.gold = gold;
			this.itens = new(itens);
		}
	}
}
