using Drafts;
using System;
using UnityEngine;

namespace GuildMasterIsekai {
	[Serializable]
	public class Spot : MonoBehaviour {
		[ReadOnly] public Vector3 lookTarget;
		[SerializeReference] IVisitorAI owner;
		public event Action<Spot, IVisitorAI> OnOwnerChanged;
		public Vector3 Position => transform.position;

		public IVisitorAI Owner { get => owner; set { owner = value; OnOwnerChanged?.Invoke(this, value); } }
	}
}

