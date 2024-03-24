using UnityEngine;
using UnityEngine.AI;

namespace GuildMasterIsekai {

	public interface IVisitorAI {
		void Act();
		bool DestinationReached { get; }
	}

	public class VisitorAI : MonoBehaviour {
		[SerializeField] NavMeshAgent agent;
		public NavMeshAgent Agent => agent;

		IVisitorAI ai;

		private void Awake() {
			enabled = false;
		}

		private void Update() {
			if(ai.DestinationReached) ai.Act();
		}

		public void SetVisitor(IVisitorAI ai) {
			enabled = true;
			this.ai = ai;
		}
	}
}

