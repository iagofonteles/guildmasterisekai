using Drafts.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Drafts.TurnBased {

	public interface ITargetedAction {
		ITargetGroup Users { get; }
		ITargetGroup Targets { get; }
		bool CanPayCost { get; }
		bool CanUse => CanPayCost && Users.HasEnough() && Targets.HasEnough();

		IEnumerable<ActionEffect> Effects(IEnumerable users, IEnumerable targets);
		IEnumerator Execution(IEnumerable users, IEnumerable targets) { yield return null; }
		Func<bool> Cancellation(IEnumerable users, IEnumerable targets) =>
			() => !Users.IsValid(users) || !Targets.IsValid(targets);

		ActionIntentionSettings GetActionIntention() => new ActionIntentionSettings(
			this, Users, Targets, Effects, Execution, Cancellation);
	}

	public interface IFixedAction {
		bool CanPayCost => true;
		bool ValidateUser(object obj) => true;
		bool ValidateTarget(object obj) => true;

		public IEnumerable<ActionEffect> Effects(IEnumerable users, IEnumerable targets) => Enumerable.Empty<ActionEffect>();
		public IEnumerator Execution(IEnumerable users, IEnumerable targets) { yield return null; }
		public Func<bool> Cancellation(IEnumerable users, IEnumerable targets) =>
			() => users.All(ValidateUser) || targets.All(ValidateTarget);

		ChainAction GetAction(object user, IEnumerable targets) => GetAction(new object[] { user }, targets);
		ChainAction GetAction(IEnumerable users, IEnumerable targets) => new(this, users,
			Effects(users, targets), Execution(users, targets), Cancellation(users, targets));
	}
}
