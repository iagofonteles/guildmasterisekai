using System;
using System.Collections;
using System.Collections.Generic;
namespace Drafts.TurnBased {

	public class Phase {
		public IRules Rules { get; }

		public event Action OnBegin, OnEnd;

		Func<IEnumerator> execute { get; }

		public Phase(IRules rules, Func<IEnumerator> execute) {
			Rules = rules;
			this.execute = execute;
		}

		public IEnumerator Execute() {
			OnBegin?.Invoke();
			yield return execute();
			OnEnd?.Invoke();
		}

		public static IEnumerator<ChainAction> ChooseAction(
			Func<IEnumerator<ActionIntentionSettings>> chooseAction,
			Func<ActionIntention, IEnumerator<IEnumerable>> chooseUsers,
			Func<ActionIntention, IEnumerator<IEnumerable>> chooseTargets) {

			ActionIntention intention = null;

		ChooseAction:
			using(var ieC = chooseAction()) {
				while(ieC.MoveNext()) yield return null;
				var settings = ieC.Current; // get command
				if(settings == null) yield break;
				intention = new(settings);
			}

		ChooseUsers:
			intention.ResetUsers();
			if(intention.UsersGroup.HasChoice) {
				using var ieT = chooseUsers(intention);
				while(ieT.MoveNext()) yield return null;
				if(ieT.Current == null) goto ChooseAction;
				intention.Users = ieT.Current; // get target
			}

			if(intention.TargetsGroup.HasChoice) {
				using var ieT = chooseTargets(intention);
				while(ieT.MoveNext()) yield return null;
				if(ieT.Current == null) goto ChooseUsers;
				intention.Targets = ieT.Current; // get target
			}

			yield return intention.GetAction();
		}
	}
}
