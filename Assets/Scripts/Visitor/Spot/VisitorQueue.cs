using System;
using System.Collections.Generic;
using Drafts;
using UnityEngine;

namespace GuildMasterIsekai.Visitor.Spot
{
    [Serializable]
    public class VisitorQueue : VisitorSpot
    {
        [SerializeField] private List<Spot> lineSpots;
        [SerializeReference, ReadOnly] private List<IVisitorAI> visitors = new();
        public bool IsFree => visitors.Count == 0 && GetFreeSpot() != null;
        public bool IsFull => !IsFree && visitors.Count >= lineSpots.Count;
        public int QueueLength => visitors.Count;

        protected override void Start()
        {
            base.Start();

            foreach (var spot in spots)
            {
                spot.OnOwnerChanged -= MoveOn;
                spot.OnOwnerChanged += MoveOn;
            }
        }

        public event Action OnDequeue;
        public Spot QueueSpot(int index) => lineSpots[index];
        public int IndexOf(IVisitorAI visitor) => visitors.IndexOf(visitor);
        public void Enqueue(IVisitorAI visitor) => visitors.Add(visitor);

        public void Dequeue(IVisitorAI visitor)
        {
            if (visitors.Remove(visitor)) OnDequeue?.Invoke();
        }

        private void MoveOn(Spot spot, IVisitorAI visitor)
        {
            if (visitor == null) OnDequeue?.Invoke();
        }
    }
}