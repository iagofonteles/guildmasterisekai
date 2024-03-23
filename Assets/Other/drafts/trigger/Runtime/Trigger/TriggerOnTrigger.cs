using UnityEngine;
using UnityEngine.Events;
namespace Drafts.Components {
    [AddComponentMenu("Drafts/Triggers/Trigger On Trigger")]
    public class TriggerOnTrigger : MonoBehaviour {
        public string targetTag;
        public UnityEvent<Collider> onTriggerEnter, onTriggerStay, onTriggerExit;
        private void OnTriggerEnter(Collider col) { if (string.IsNullOrEmpty(targetTag) || col.CompareTag(targetTag)) onTriggerEnter.Invoke(col); }
        private void OnTriggerStay(Collider col) { if (string.IsNullOrEmpty(targetTag) || col.CompareTag(targetTag)) onTriggerStay.Invoke(col); }
        private void OnTriggerExit(Collider col) { if (string.IsNullOrEmpty(targetTag) || col.CompareTag(targetTag)) onTriggerExit.Invoke(col); }
    }
}