using UnityEngine;
using UnityEngine.Events;
namespace Drafts.Components {
	public class TriggerSwitch : MonoBehaviour {

		public UnityEvent<bool> OnChanged;
		public UnityEngine.Events.UnityEvent OnEnabled;
		public UnityEngine.Events.UnityEvent OnDisabled;

		bool _value;

		public bool Value {
			get => _value;
			set {
				if(_value == value) return;
				_value = value;
				OnChanged?.Invoke(value);
				(value ? OnEnabled : OnDisabled)?.Invoke();
			}
		}
	}
}
