using System;
using UnityEngine;

namespace GuildMasterIsekai.Visitor.Spot {
	[Serializable]
	public class HallSpots {
		[SerializeField] private Transform entrance;
		[SerializeField] private Transform exit;
		[SerializeField] private VisitorQueue counter;
		[SerializeField] private VisitorQueue office;
		[SerializeField] private VisitorSpot board;
		[SerializeField] private VisitorSpot table;

		public Vector3 Entrance => entrance.position;
		public Vector3 Exit => exit.position;
		public VisitorQueue Counter => counter;
		public VisitorQueue Office => office;
		public VisitorSpot Board => board;
		public VisitorSpot Table => table;
	}
}

