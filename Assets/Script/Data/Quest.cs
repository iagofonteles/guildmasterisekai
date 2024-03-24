using System;
using UnityEngine;

namespace GuildMasterIsekai {
	[Serializable]
	public class Quest : IGuid {
		[SerializeField] string guid;
		public string Guid => guid;
		public Quest() {

		}
	}
}
