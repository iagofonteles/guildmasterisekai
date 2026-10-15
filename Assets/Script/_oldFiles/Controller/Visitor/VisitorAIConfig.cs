using System;
using UnityEngine;

namespace GuildMasterIsekai {
	[CreateAssetMenu(menuName = "Guild Master/VisitorAIConfig")]
	public class VisitorAIConfig : ScriptableObject {
		[SerializeField] private CostumerConf freelancer;
		[SerializeField] private CostumerConf costumer;

		public CostumerConf Costumer => costumer;

		[Serializable]
		public class CostumerConf {
			public float sitTime = 15;
		}
	}
}

