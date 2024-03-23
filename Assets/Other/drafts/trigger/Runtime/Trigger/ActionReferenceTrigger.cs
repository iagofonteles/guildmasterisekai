#if ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Drafts.Components {
	public class ActionReferenceTrigger : MonoBehaviour {
		[SerializeField] InputActionReference action;
		public UnityEvent onTrigger; 

		void Awake() => action.action.Enable();
		void OnEnable() => action.action.started += Trigger;
		void OnDisable() => action.action.started -= Trigger;
		private void Trigger(InputAction.CallbackContext obj) => onTrigger.Invoke();
	}
}
#endif