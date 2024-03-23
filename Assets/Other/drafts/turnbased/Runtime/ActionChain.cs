using System;
using System.Collections;
using System.Collections.Generic;

namespace Drafts.TurnBased {
	public delegate IEnumerator AnimationHandler(ChainAction entry);

	public class ActionChain {
		Stack<ChainAction> stack = new();
		Action update;
		public event Action<ChainAction> OnAdded;
		public event Action<ChainAction> OnEntryEvaluating;
		public event Action<ChainAction> OnEntryEvaluated;

		public HashSet<AnimationHandler> EffectHandlers { get; } = new();
		public Func<ChainAction, IEnumerator> ExecutionHandler { get; set; } = DefautExecutionHandler;
		public ActionChain(Action update) => this.update = update;

		public void Add(IEnumerator execution) => Add(new ChainAction(execution));
		public void Add(Action effect) => Add(new ChainAction(null, null, new ActionEffect[] { new(effect) }));
		public void Add(object source, Action effect) => Add(new ChainAction(source, null, new ActionEffect[] { new(effect) }));

		public void Add(ChainAction action) {
			stack.Push(action);
			OnAdded?.Invoke(action);
		}

		public void Clear() => stack.Clear();

		public IEnumerator Evaluate() {
			while(stack.TryPop(out var entry)) {
				if(entry.Cancelled) continue;

				OnEntryEvaluating?.Invoke(entry);
				yield return ExecutionHandler(entry);

				foreach(var handler in EffectHandlers)
					yield return handler(entry);

				foreach(var effect in entry.Effects) effect.Apply();
				OnEntryEvaluated?.Invoke(entry);
				update?.Invoke();
			}
		}

		static IEnumerator DefautExecutionHandler(ChainAction action) {
			yield return action.Execution;
		}
	}
}
