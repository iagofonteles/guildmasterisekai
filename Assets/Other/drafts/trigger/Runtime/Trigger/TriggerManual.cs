using UnityEngine;
using UnityEngine.Events;

namespace Drafts.Components {
    [AddComponentMenu("Drafts/Triggers/Trigger Manual")]
    public class TriggerManual : MonoBehaviour {
        public UnityEvent onTrigger;
        public UnityEvent<int> onTriggerInt;
        public UnityEvent<float> onTriggerFloat;
        public UnityEvent<bool> onTriggerBool;
        public UnityEvent<string> onTriggerString;
        public UnityEvent<GameObject> onTriggerGameObject;
        public UnityEvent<Component> onTriggerComponent;

        public void Trigger() => onTrigger.Invoke();
        public void Trigger(int value) => onTriggerInt.Invoke(value);
        public void Trigger(float value) => onTriggerFloat.Invoke(value);
        public void Trigger(bool value) => onTriggerBool.Invoke(value);
        public void Trigger(string value) => onTriggerString.Invoke(value);
        public void Trigger(GameObject value) => onTriggerGameObject.Invoke(value);
        public void Trigger(Component value) => onTriggerComponent.Invoke(value);

        //public UnityEvent<object> onTriggerObj;
        //public void Trigger() => onTrigger.Invoke();
        //public void Trigger(int value) => onTriggerObj.Invoke(value);
        //public void Trigger(float value) => onTriggerObj.Invoke(value);
        //public void Trigger(bool value) => onTriggerObj.Invoke(value);
        //public void Trigger(string value) => onTriggerObj.Invoke(value);
        //public void Trigger(GameObject value) => onTriggerObj.Invoke(value);
        //public void Trigger(Component value) => onTriggerObj.Invoke(value);
    }
}