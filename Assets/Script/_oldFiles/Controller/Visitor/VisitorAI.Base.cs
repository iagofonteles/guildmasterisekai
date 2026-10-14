using Drafts;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace GuildMasterIsekai {
	public abstract class BaseVisitorAI : IVisitorAI {
		protected NavMeshAgent Agent { get; }
		protected HallSpots Spots { get; }
		protected Action nextAction;
		protected Func<bool> meta;
		protected float timeout;
		protected Spot lastSpot;

		Coroutine lookRoutine;

		public BaseVisitorAI(NavMeshAgent agent, HallSpots spots) {
			Agent = agent;
			Spots = spots;
		}

		public bool ReadyToAct => meta();
		public void Act() => nextAction();

		protected bool None() => false;
		protected bool WaitTimeout() => (timeout -= Time.deltaTime) <= 0;
		protected bool ReachDestination() => Vector3.Distance(
			Agent.destination, Agent.transform.position) < .5f;

		protected void WalkTo(Spot spot, Action next) {
			lookRoutine?.Stop();
			WalkTo(spot.Position, () => {
				LookAt(spot.lookTarget);
				next();
			});
			OcupySpot(spot);
		}

		protected void WalkTo(Vector3 position, Action next) {
			LeaveSpot();
			Agent.SetDestination(position);
			meta = ReachDestination;
			nextAction = next;
		}

		protected void Wait(float time, Action next) {
			timeout = time;
			meta = WaitTimeout;
			nextAction = next;
		}

		protected void Stop() => meta = None;

		protected void WaitInQueue(VisitorQueue queue, Action next) {
			var pos = queue.QueueLength + 1;
			if(queue.IsFree) { Proceed(); return; }

			queue.Enqueue(this);
			queue.OnDequeue += MoveOn;
			MoveOn();

			void MoveOn() {
				if(pos-- == 0) Proceed();
				else WalkTo(queue.QueueSpot(pos), Stop);
			}

			void Proceed() {
				queue.Dequeue(this);
				queue.OnDequeue -= MoveOn;
				WalkTo(queue.GetFreeSpot(), next);
			}
		}

		protected void Leave() => WalkTo(Spots.Exit, Destroy);
		protected virtual void Destroy() => UnityEngine.Object.Destroy(Agent.gameObject);

		public void Dispose() {
			LeaveSpot();
			lookRoutine?.Stop();
		}

		protected void LeaveSpot() {
			if(lastSpot == null) return;
			lastSpot.Owner = null;
			lastSpot = null;
		}

		protected void OcupySpot(Spot spot) {
			LeaveSpot();
			spot.Owner = this;
			lastSpot = spot;
		}

		protected Coroutine LookAt(Vector3 point) => lookRoutine = _LookAt(point, .3f).Start();
		IEnumerator _LookAt(Vector3 point, float duration) {
			var direction = (point - Agent.transform.position).normalized;
			var target = Quaternion.LookRotation(direction, Agent.transform.up);
			var current = Agent.transform.rotation;
			for(var i = 0f; i < 1; i += Time.deltaTime / duration)
				yield return Agent.transform.rotation = Quaternion.Slerp(current, target, i);
		}
	}
}

