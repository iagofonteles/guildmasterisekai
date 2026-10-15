using System;
using System.Collections;
using GuildMasterIsekai.Visitor.Spot;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

namespace GuildMasterIsekai.Visitor
{
    public abstract class BaseVisitorAI : IVisitorAI
    {
        private readonly MonoBehaviour runner;
        private bool disposed;
        protected Spot.Spot lastSpot;
        private Action leaveQueue;

        private Coroutine lookRoutine;
        protected Func<bool> meta;
        protected Action nextAction;
        protected float timeout;

        public BaseVisitorAI(NavMeshAgent agent, HallSpots spots)
        {
            Agent = agent;
            Spots = spots;
            runner = agent.GetComponent<VisitorAI>();
            meta = None;
        }

        protected NavMeshAgent Agent { get; }
        protected HallSpots Spots { get; }

        public bool ReadyToAct => !disposed && meta();

        public void Act()
        {
            if (disposed) return;
            var action = nextAction;
            Stop();
            action?.Invoke();
        }

        public virtual void Dispose()
        {
            if (disposed) return;
            disposed = true;
            leaveQueue?.Invoke();
            LeaveSpot();
            StopLooking();
        }

        protected bool None() => false;
        protected bool WaitTimeout() => (timeout -= Time.deltaTime) <= 0;

        protected bool ReachDestination() => Agent.isOnNavMesh &&
                                             !Agent.pathPending &&
                                             Agent.pathStatus == NavMeshPathStatus.PathComplete &&
                                             Agent.remainingDistance <= Mathf.Max(Agent.stoppingDistance, .05f);

        protected void WalkTo(Spot.Spot spot, Action next)
        {
            StopLooking();
            var previous = lastSpot;
            lastSpot = spot;
            spot.Owner = this;
            Agent.SetDestination(spot.Position);
            meta = ReachDestination;
            nextAction = () =>
            {
                LookAt(spot.lookTarget);
                next();
            };
            if (previous && previous != spot && previous.Owner == this) previous.Owner = null;
        }

        protected void WalkTo(Vector3 position, Action next)
        {
            StopLooking();
            LeaveSpot();
            Agent.SetDestination(position);
            meta = ReachDestination;
            nextAction = next;
        }

        protected void Wait(float time, Action next)
        {
            timeout = time;
            meta = WaitTimeout;
            nextAction = next;
        }

        protected void Stop() => meta = None;

        protected void WaitInQueue(VisitorQueue queue, Action next)
        {
            leaveQueue?.Invoke();
            queue.Enqueue(this);
            leaveQueue = () =>
            {
                queue.OnDequeue -= MoveOn;
                leaveQueue = null;
                queue.Dequeue(this);
            };
            queue.OnDequeue += MoveOn;
            MoveOn();

            void MoveOn()
            {
                if (disposed) return;
                var pos = queue.IndexOf(this);
                var spot = pos == 0 ? queue.GetFreeSpot() : null;

                if (spot != null)
                {
                    queue.OnDequeue -= MoveOn;
                    leaveQueue = null;
                    WalkTo(spot, next);
                    queue.Dequeue(this);
                }
                else if (pos >= 0)
                {
                    WalkTo(queue.QueueSpot(pos), Stop);
                }
            }
        }

        protected void Leave() => WalkTo(Spots.Exit, Destroy);

        protected virtual void Destroy()
        {
            Dispose();
            Object.Destroy(Agent.gameObject);
        }

        protected void LeaveSpot()
        {
            if (!lastSpot) return;
            var spot = lastSpot;
            lastSpot = null;
            if (spot.Owner == this) spot.Owner = null;
        }

        protected void OcupySpot(Spot.Spot spot)
        {
            LeaveSpot();
            spot.Owner = this;
            lastSpot = spot;
        }

        private void StopLooking()
        {
            if (lookRoutine != null && runner) runner.StopCoroutine(lookRoutine);
            lookRoutine = null;
        }

        protected Coroutine LookAt(Vector3 point) => lookRoutine = runner.StartCoroutine(_LookAt(point, .3f));

        private IEnumerator _LookAt(Vector3 point, float duration)
        {
            var direction = (point - Agent.transform.position).normalized;
            if (direction.sqrMagnitude == 0) yield break;
            var target = Quaternion.LookRotation(direction, Agent.transform.up);
            var current = Agent.transform.rotation;
            for (var i = 0f; i < 1; i += Time.deltaTime / duration)
                yield return Agent.transform.rotation = Quaternion.Slerp(current, target, i);
            Agent.transform.rotation = target;
        }
    }
}