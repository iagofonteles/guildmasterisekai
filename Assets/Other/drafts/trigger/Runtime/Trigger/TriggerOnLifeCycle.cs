using UnityEngine;
using UnityEngine.Events;

namespace Drafts.Components {
    [AddComponentMenu("Drafts/Triggers/Trigger On Life Cycle")]
    public class TriggerOnLifeCycle : MonoBehaviour {
        public UnityEvent onAwake;
        public UnityEvent onStart;
        public UnityEvent onEnable;
        public UnityEvent onDisable;
        public UnityEvent onDestroy;

        private void Awake() => onAwake.Invoke();
        private void Start() => onStart.Invoke();
        private void OnEnable() => onEnable.Invoke();
        private void OnDisable() => onDisable.Invoke();
        private void OnDestroy() => onDestroy.Invoke();
    }
}
