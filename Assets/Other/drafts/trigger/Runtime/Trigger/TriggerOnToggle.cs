using UnityEngine;
using UnityEngine.Events;

public class TriggerOnToggle : MonoBehaviour {
	[SerializeField] bool value;

	public UnityEvent<bool> OnChanged;
	public UnityEvent<bool> OnInverse;
	public UnityEvent OnActivate;
	public UnityEvent OnDeactivate;

	public void Toggle() => value = !value;
	public bool Vaue {
		get => value;
		set {
			if(this.value == value) return;
			this.value = value;
			OnChanged.Invoke(value);
			OnInverse.Invoke(!value);
			if(value) OnActivate.Invoke();
			else OnDeactivate.Invoke();
		}
	}
}
