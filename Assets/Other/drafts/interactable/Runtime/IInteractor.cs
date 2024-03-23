using System;
using UnityEngine;

public interface IInteractor {
	Vector3 Position { get; }

	public IInteractable Current { get; protected set; }
	public Action<IInteractable> OnCurrentChanged { get; set; }

	public void Search() {
		var next = IInteractable.Nearest(this);
		if(next == Current) return;
		Current?.RemoveInteractor(this);
		Current = next;
		Current?.AddInteractor(this);
		OnCurrentChanged?.Invoke(Current);
	}

	sealed void Trigger() => Current?.Trigger(this);
	sealed void Cancel() => Current?.Cancel(this);
}
