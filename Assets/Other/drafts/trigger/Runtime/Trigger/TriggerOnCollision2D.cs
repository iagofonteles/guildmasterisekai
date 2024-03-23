using UnityEngine;
using UnityEngine.Events;
namespace Drafts.Components {
	[AddComponentMenu("Drafts/Triggers/Trigger On Collision")]
    public class TriggerOnCollision2D : MonoBehaviour {
        public string targetTag;
        public UnityEvent<Collision2D> onCollisionEnter2D, onCollisionStay2D, onCollisionExit2D;
        private void OnCollisionEnter2D(Collision2D col) { if (string.IsNullOrEmpty(targetTag) || col.collider.CompareTag(targetTag)) onCollisionEnter2D.Invoke(col); }
        private void OnCollisionStay2D(Collision2D col) { if (string.IsNullOrEmpty(targetTag) || col.collider.CompareTag(targetTag)) onCollisionStay2D.Invoke(col); }
        private void OnCollisionExit2D(Collision2D col) { if (string.IsNullOrEmpty(targetTag) || col.collider.CompareTag(targetTag)) onCollisionExit2D.Invoke(col); }
    }
}