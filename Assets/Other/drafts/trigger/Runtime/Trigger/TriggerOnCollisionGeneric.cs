using UnityEngine;
using UnityEngine.Events;
namespace Drafts.Components {
	public class TriggerOnCollision<T> : MonoBehaviour {
		public string targetTag;
		public UnityEvent<T> onCollisionEnter, onCollisionStay, onCollisionExit;
		private void OnCollisionEnter(Collision col) { if((string.IsNullOrEmpty(targetTag) || col.collider.CompareTag(targetTag)) && col.collider.TryGetComponent<T>(out var c)) onCollisionEnter.Invoke(c); }
		private void OnCollisionStay(Collision col) { if((string.IsNullOrEmpty(targetTag) || col.collider.CompareTag(targetTag)) && col.collider.TryGetComponent<T>(out var c)) onCollisionStay.Invoke(c); }
		private void OnCollisionExit(Collision col) { if((string.IsNullOrEmpty(targetTag) || col.collider.CompareTag(targetTag)) && col.collider.TryGetComponent<T>(out var c)) onCollisionExit.Invoke(c); }
	}
	public class TriggerOnCollision2D<T> : MonoBehaviour {
		public string targetTag;
		public UnityEvent<T> onCollisionEnter2D, onCollisionStay2D, onCollisionExit2D;
		private void OnCollisionEnter2D(Collision2D col) { if((string.IsNullOrEmpty(targetTag) || col.collider.CompareTag(targetTag)) && col.collider.TryGetComponent<T>(out var c)) onCollisionEnter2D.Invoke(c); }
		private void OnCollisionStay2D(Collision2D col) { if((string.IsNullOrEmpty(targetTag) || col.collider.CompareTag(targetTag)) && col.collider.TryGetComponent<T>(out var c)) onCollisionStay2D.Invoke(c); }
		private void OnCollisionExit2D(Collision2D col) { if((string.IsNullOrEmpty(targetTag) || col.collider.CompareTag(targetTag)) && col.collider.TryGetComponent<T>(out var c)) onCollisionExit2D.Invoke(c); }
	}
}