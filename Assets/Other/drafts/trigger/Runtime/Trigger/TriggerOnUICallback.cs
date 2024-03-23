using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Drafts.Components {

    public class TriggerOnUICallback : UIBehaviour {
        public UnityEvent onRectChanged;
        protected override void OnRectTransformDimensionsChange() => onRectChanged.Invoke();
    }
}