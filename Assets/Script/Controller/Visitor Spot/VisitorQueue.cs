using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using Drafts;

namespace GuildMasterIsekai {

	[Serializable]
	public class VisitorQueue : VisitorSpot {
		[SerializeField] List<Spot> lineSpots;
		[SerializeReference, ReadOnly] List<IVisitorAI> visitors = new();

		public event Action OnDequeue;
		public bool IsFree => visitors.Count == 0 && spots.Any(s => s.Owner == null);
		public bool IsFull => visitors.Count == lineSpots.Count;
		public int QueueLength => visitors.Count;
		public Spot QueueSpot(int index) => lineSpots[index];
		public void Enqueue(IVisitorAI visitor) => visitors.Add(visitor);
		public void Dequeue(IVisitorAI visitor) => visitors.Remove(visitor);

		protected override void Start() {
			base.Start();
			foreach(var spot in spots) {
				spot.OnOwnerChanged -= MoveOn;
				spot.OnOwnerChanged += MoveOn;
			}
		}

		void MoveOn(Spot spot, IVisitorAI visitor) {
			if(visitor == null) OnDequeue?.Invoke();
		}
	}
}

