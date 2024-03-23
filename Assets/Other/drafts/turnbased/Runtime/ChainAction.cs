using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Drafts.TurnBased {

	public class ChainAction {

		Func<bool> cancellation;

		public object Source { get; }
		public IEnumerable Users { get; }
		public IEnumerable<ActionEffect> Effects { get; }
		public IEnumerator Execution { get; }
		public bool Cancelled => cancellation?.Invoke() ?? false;
		public event Action OnCancelled;

		public ChainAction(
			object source,
			IEnumerable users,
			IEnumerable<ActionEffect> effects,
			IEnumerator execute = null,
			Func<bool> cancellation = null
			) {
			this.cancellation = cancellation;
			Source = source;
			Users = users;
			Effects = effects;
			Execution = execute;

			foreach(var effect in effects)
				effect.Source = source;
		}


		public ChainAction(
			object source,
			ActionEffect effect,
			IEnumerator execute = null,
			Func<bool> cancellation = null
			) {
			this.cancellation = cancellation;
			Source = source;
			Users = effect.Users;
			Effects = new ActionEffect[] { effect };
			Execution = execute;
			effect.Source = source;
		}

		public ChainAction(IEnumerator execution) {
			Execution = execution;
			Effects = Enumerable.Empty<ActionEffect>();
		}

		public ChainAction(Action effect) => Effects = new ActionEffect[] { new(effect) };

		public ChainAction SetCancellation(Func<bool> c) { cancellation = c; return this; }
		public void Cancel() { cancellation = ReturnTrue; OnCancelled?.Invoke(); }

		static bool ReturnTrue() => true;
	}

}