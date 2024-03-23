using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IInteractable {

	static List<IInteractable> _all { get; } = new();
	public static IEnumerable<IInteractable> All => _all;

	public static IInteractable Nearest(IInteractor interactor) => _all.Where(i => i.InRange(interactor))
		.OrderBy(i => (i.Position - interactor.Position).sqrMagnitude).FirstOrDefault();

	Vector3 Position { get; }
	void Trigger(IInteractor interactor);
	void Cancel(IInteractor interactor) { }
	bool InRange(IInteractor interactor);

	sealed void Enable() => _all.Add(this);
	sealed void Disable() => _all.Remove(this);

	protected HashSet<IInteractor> _interactors { get; }
	sealed IEnumerable<IInteractor> Interactors => _interactors;
	Action<IEnumerable<IInteractor>> OnInteractorsChanged { get; set; }

	public void AddInteractor(IInteractor interactor) {
		if(_interactors.Add(interactor))
			OnInteractorsChanged?.Invoke(_interactors);
	}
	public void RemoveInteractor(IInteractor interactor) {
		if(_interactors.Remove(interactor))
			OnInteractorsChanged?.Invoke(_interactors);
	}

}
