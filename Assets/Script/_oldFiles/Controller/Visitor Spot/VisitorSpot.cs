using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GuildMasterIsekai {

	[Serializable]
	public class VisitorSpot : MonoBehaviour {
		[SerializeField] protected List<Spot> spots = new();

		public Vector3 Position => transform.position;
		public Spot GetFreeSpot() => spots.FirstOrDefault(
			s => s.isActiveAndEnabled && s.Owner == null);

		protected virtual void Start() {
			foreach(var spot in spots)
				spot.lookTarget = Position;
		}
	}
}

