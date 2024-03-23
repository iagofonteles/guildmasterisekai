using UnityEngine;
using UnityEngine.Events;
namespace Drafts.Components {
	[AddComponentMenu("Drafts/Triggers/Trigger On Collision")]
    public class TriggerOnCollision : MonoBehaviour {
        public string targetTag;
        public UnityEvent<Collision> onCollisionEnter, onCollisionStay, onCollisionExit;
        private void OnCollisionEnter(Collision col) { if (string.IsNullOrEmpty(targetTag) || col.collider.CompareTag(targetTag)) onCollisionEnter.Invoke(col); }
        private void OnCollisionStay(Collision col) { if (string.IsNullOrEmpty(targetTag) || col.collider.CompareTag(targetTag)) onCollisionStay.Invoke(col); }
        private void OnCollisionExit(Collision col) { if (string.IsNullOrEmpty(targetTag) || col.collider.CompareTag(targetTag)) onCollisionExit.Invoke(col); }
    }
}