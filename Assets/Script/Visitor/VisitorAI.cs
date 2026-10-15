using System;
using UnityEngine;
using UnityEngine.AI;

namespace GuildMasterIsekai.Visitor {

	public interface IVisitorAI : IDisposable {
		void Act();
		bool ReadyToAct { get; }
	}

	public class VisitorAI : MonoBehaviour {
		[SerializeField] private NavMeshAgent agent;
		public NavMeshAgent Agent => agent;

		private IVisitorAI ai;

		private void Awake() {
			enabled = false;
		}

		private void Update() {
			if(ai != null && ai.ReadyToAct) ai.Act();
		}

		public void SetVisitor(IVisitorAI ai) {
			if(ReferenceEquals(this.ai, ai)) return;
			this.ai?.Dispose();
			this.ai = ai;
			enabled = ai != null;
		}

		private void OnDestroy() {
			ai?.Dispose();
		}
	}
}

