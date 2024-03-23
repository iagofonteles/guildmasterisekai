using UnityEngine;
using UnityEngine.Events;

namespace Drafts.Components {
	public class TriggerOnFacing : MonoBehaviour {

		public string targetTag = "Player";
		public float angle = 45f;
		public UnityEvent<GameObject> trigger;

		private void OnTriggerStay(Collider other) {
			if(targetTag != "" && !other.CompareTag(targetTag)) return;
			var angleBetween = Vector3.Angle(-transform.forward, other.transform.forward);
			if(angleBetween > angle / 2) return;
			trigger.Invoke(other.gameObject);
		}
	}
}
