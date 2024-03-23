using UnityEngine;
using UnityEngine.Events;
namespace Drafts.Components {
	public class TriggerOnTrigger<T> : MonoBehaviour {
		public string targetTag;
		public UnityEvent<T> onTriggerEnter, onTriggerStay, onTriggerExit;
		private void OnTriggerEnter(Collider col) { if((string.IsNullOrEmpty(targetTag) || col.CompareTag(targetTag)) && col.TryGetComponent<T>(out var c)) onTriggerEnter.Invoke(c); }
		private void OnTriggerStay(Collider col) { if((string.IsNullOrEmpty(targetTag) || col.CompareTag(targetTag)) && col.TryGetComponent<T>(out var c)) onTriggerStay.Invoke(c); }
		private void OnTriggerExit(Collider col) { if((string.IsNullOrEmpty(targetTag) || col.CompareTag(targetTag)) && col.TryGetComponent<T>(out var c)) onTriggerExit.Invoke(c); }
	}
	public class TriggerOnTrigger2D<T> : MonoBehaviour {
		public string targetTag;
		public UnityEvent<T> onTriggerEnter2D, onTriggerStay2D, onTriggerExit2D;
		private void OnTriggerEnter2D(Collider2D col) { if((string.IsNullOrEmpty(targetTag) || col.CompareTag(targetTag)) && col.TryGetComponent<T>(out var c)) onTriggerEnter2D.Invoke(c); }
		private void OnTriggerStay2D(Collider2D col) { if((string.IsNullOrEmpty(targetTag) || col.CompareTag(targetTag)) && col.TryGetComponent<T>(out var c)) onTriggerStay2D.Invoke(c); }
		private void OnTriggerExit2D(Collider2D col) { if((string.IsNullOrEmpty(targetTag) || col.CompareTag(targetTag)) && col.TryGetComponent<T>(out var c)) onTriggerExit2D.Invoke(c); }
	}
}
