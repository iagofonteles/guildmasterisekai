using System;
using UnityEngine;

namespace GuildMasterIsekai {
	[CreateAssetMenu(menuName = "Guild Master/VisitorAIConfig")]
	public class VisitorAIConfig : ScriptableObject {
		[SerializeField] CostumerConf freelancer;
		[SerializeField] CostumerConf costumer;

		public CostumerConf Costumer => costumer;

		[Serializable]
		public class CostumerConf {
			public float sitTime = 15;
		}
	}
}

