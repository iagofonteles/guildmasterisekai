using System;
using UnityEngine;

namespace GuildMasterIsekai {
	[Serializable]
	public class HallSpots {
		[SerializeField] Transform entrance;
		[SerializeField] Transform exit;
		[SerializeField] VisitorQueue counter;
		[SerializeField] VisitorQueue office;
		[SerializeField] VisitorSpot board;
		[SerializeField] VisitorSpot table;

		public Vector3 Entrance => entrance.position;
		public Vector3 Exit => exit.position;
		public VisitorQueue Counter => counter;
		public VisitorQueue Office => office;
		public VisitorSpot Board => board;
		public VisitorSpot Table => table;
	}
}

