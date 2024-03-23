using UnityEngine;
namespace Drafts.Components {
	public abstract class TriggerBase : MonoBehaviour {
		public abstract void Trigger();
	}
	//[AddComponentMenu("Drafts/Trigger/Basic Trigger")]
	//public class OnTrigger : MonoBehaviour {
	//	public UnityEvent onTrigger;
	//	public virtual void Trigger() => onTrigger.Invoke();
	//}
}