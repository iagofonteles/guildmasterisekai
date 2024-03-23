using Drafts;
using UnityEngine;
using UnityEngine.Events;

public class BoolView : DataView<bool> {
	[SerializeField] UnityEvent<bool> onValue;
	[SerializeField] UnityEvent<bool> onInverse;
	[SerializeField] UnityEvent onTrue;
	[SerializeField] UnityEvent onFalse;
	protected override void Repaint() {
		onValue.Invoke(Data);
		onInverse.Invoke(!Data);
		if(Data) onTrue.Invoke();
		else onFalse.Invoke();
	}
}
