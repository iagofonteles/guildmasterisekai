using UnityEngine;
using UnityEngine.Events;

namespace Drafts.Components {
	[AddComponentMenu("Drafts/Trigger/Trigger On Activation")]
	public class TriggerOnActivation : MonoBehaviour {
		public UnityEvent<bool> onTrigger;
		public UnityEvent<bool> onTriggerInverse;
		public UnityEvent onTriggerOn;
		public UnityEvent onTriggerOff;
		public void Trigger(bool value) {
			onTrigger.Invoke(value);
			onTriggerInverse.Invoke(!value);
			if(value) onTriggerOn.Invoke();
			else onTriggerOff.Invoke();
		}
	}
}