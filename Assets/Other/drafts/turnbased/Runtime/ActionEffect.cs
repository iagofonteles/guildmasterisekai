using System;
using System.Collections;
using System.Linq;

namespace Drafts.TurnBased {

	public class ActionEffect {
		bool applied;
		Action<object> Action { get; }

		public object Source { get; internal set; }
		public IEnumerable Users { get; }
		public IEnumerable Targets { get; }
		public bool Cancelled { get; private set; }
		public event Action OnCancelled;

		public ActionEffect(Action action) : this(null, null, _ => action()) { }
		public ActionEffect(IEnumerable users, Action action) : this(users, null, _ => action()) { }
		public ActionEffect(object user, object target, Action<object> action)
			: this(Enumerable.Repeat(user, 1), Enumerable.Repeat(target, 1), action) { }
		public ActionEffect(IEnumerable users, IEnumerable targets, Action<object> action) {
			Users = users;
			Targets = targets;
			Action = action;
		}

		public void Apply() {
			if(applied || Cancelled) return;
			applied = true;

			if(Targets == null) Action(null);
			else foreach(var t in Targets) Action(t);
		}

		public void Cancel() { Cancelled = true; OnCancelled?.Invoke(); }

	}
}
