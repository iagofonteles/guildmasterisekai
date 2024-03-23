using UnityEngine;
using UnityEngine.Events;
namespace Drafts.Components {
    [AddComponentMenu("Drafts/Triggers/Trigger On Trigger")]
    public class TriggerOnTrigger2D : MonoBehaviour {
        public string targetTag;
        public UnityEvent<Collider2D> onTriggerEnter2D, onTriggerStay2D, onTriggerExit2D;
        private void OnTriggerEnter2D(Collider2D col) { if (string.IsNullOrEmpty(targetTag) || col.CompareTag(targetTag)) onTriggerEnter2D.Invoke(col); }
        private void OnTriggerStay2D(Collider2D col) { if (string.IsNullOrEmpty(targetTag) || col.CompareTag(targetTag)) onTriggerStay2D.Invoke(col); }
        private void OnTriggerExit2D(Collider2D col) { if (string.IsNullOrEmpty(targetTag) || col.CompareTag(targetTag)) onTriggerExit2D.Invoke(col); }
    }

}