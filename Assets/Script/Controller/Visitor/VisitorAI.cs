using System;
using UnityEngine;
using UnityEngine.AI;

namespace GuildMasterIsekai {

	public interface IVisitorAI : IDisposable {
		void Act();
		bool ReadyToAct { get; }
	}

	public class VisitorAI : MonoBehaviour {
		[SerializeField] NavMeshAgent agent;
		public NavMeshAgent Agent => agent;

		IVisitorAI ai;

		private void Awake() {
			enabled = false;
		}

		private void Update() {
			if(ai.ReadyToAct) ai.Act();
		}

		public void SetVisitor(IVisitorAI ai) {
			enabled = true;
			this.ai = ai;
		}

		private void OnDestroy() {
			ai?.Dispose();
		}
	}
}

