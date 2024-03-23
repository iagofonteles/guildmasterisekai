#if ENABLE_LEGACY_INPUT_MANAGER 
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Drafts.Components {

    /// <summary>Only works with old input system.</summary>
    [RequireComponent(typeof(Collider))]
    public class TriggerOnClickCollider : MonoBehaviour {
        public UnityEvent onMouseDown;
        public UnityEvent onMouseUp;
        public UnityEvent onMouseClick;
        public UnityEvent onMouseEnter;
        public UnityEvent onMouseExit;
        public UnityEvent onMouseOver;
        public UnityEvent onMouseDrag;

        bool NotBlocked => enabled && !EventSystem.current.IsPointerOverGameObject();

        private void OnMouseDown() { if (NotBlocked) onMouseDown.Invoke(); }
        private void OnMouseUp() { if (NotBlocked) onMouseUp.Invoke(); }
        private void OnMouseUpAsButton() { if (NotBlocked) onMouseClick.Invoke(); }
        private void OnMouseEnter() { if (NotBlocked) onMouseEnter.Invoke(); }
        private void OnMouseExit() { if (NotBlocked) onMouseExit.Invoke(); }
        private void OnMouseOver() { if (NotBlocked) onMouseOver.Invoke(); }
        private void OnMouseDrag() { if (NotBlocked) onMouseDrag.Invoke(); }
    }
}
#endif
