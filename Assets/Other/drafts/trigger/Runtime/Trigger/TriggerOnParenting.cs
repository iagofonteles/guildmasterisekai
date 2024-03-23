using UnityEngine;
using UnityEngine.Events;

namespace Drafts.Components {

	[AddComponentMenu("Drafts/Triggers/Trigger On Parenting")]
	public class TriggerOnParenting : MonoBehaviour {

		public UnityEvent<Transform> onParentChanged;
		[Tooltip("When childCount == 0")]
		public UnityEvent onTransformEmpty;
		public UnityEvent<int> onChildrenChanged;

		private void OnTransformParentChanged() => onParentChanged.Invoke(transform.parent);

		private void OnTransformChildrenChanged() {
			if(transform.childCount == 0) onTransformEmpty.Invoke();
			onChildrenChanged.Invoke(transform.childCount);
		}
	}
}