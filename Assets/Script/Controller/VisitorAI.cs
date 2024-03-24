using UnityEngine;
using UnityEngine.AI;

namespace GuildMasterIsekai {
	public class VisitorAI : MonoBehaviour {
		[SerializeField] NavMeshAgent agent;
		IVisitor visitor;

		private void Awake() {
			enabled = false;
		}

		public void SetVisitor(IVisitor visitor, Transform balcon) {
			enabled = true;
			this.visitor = visitor;
			agent.SetDestination(balcon.position);
		}

		//private void Update() {

		//}
	}
}

