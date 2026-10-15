using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using Drafts;

namespace GuildMasterIsekai {

	[Serializable]
	public class VisitorQueue : VisitorSpot {
		[SerializeField] private List<Spot> lineSpots;
		[SerializeReference, ReadOnly] private List<IVisitorAI> visitors = new();

		public event Action OnDequeue;
		public bool IsFree => visitors.Count == 0 && GetFreeSpot() != null;
		public bool IsFull => !IsFree && visitors.Count >= lineSpots.Count;
		public int QueueLength => visitors.Count;
		public Spot QueueSpot(int index) => lineSpots[index];
		public int IndexOf(IVisitorAI visitor) => visitors.IndexOf(visitor);
		public void Enqueue(IVisitorAI visitor) => visitors.Add(visitor);
		public void Dequeue(IVisitorAI visitor) {
			if(visitors.Remove(visitor)) OnDequeue?.Invoke();
		}

		protected override void Start() {
			base.Start();
			foreach(var spot in spots) {
				spot.OnOwnerChanged -= MoveOn;
				spot.OnOwnerChanged += MoveOn;
			}
		}

		private void MoveOn(Spot spot, IVisitorAI visitor) {
			if(visitor == null) OnDequeue?.Invoke();
		}
	}
}

