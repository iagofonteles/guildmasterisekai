using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Drafts.TurnBased {

	public class ActionIntentionSettings {
		public delegate IEnumerable<ActionEffect> GetEffectsHanldler(IEnumerable users, IEnumerable targets);
		public delegate IEnumerator GetExecutionHanldler(IEnumerable users, IEnumerable targets);
		public delegate Func<bool> GetCancellationHanldler(IEnumerable users, IEnumerable targets);

		public object Source { get; }
		public ITargetGroup UsersGroup { get; }
		public ITargetGroup TargetsGroup { get; }
		public GetEffectsHanldler GetEffects { get; }
		public GetExecutionHanldler GetExecution { get; }
		public GetCancellationHanldler GetCancellation { get; }

		public ActionIntentionSettings(
			object source = default,
			ITargetGroup userGroup = default,
			ITargetGroup targetGroup = default,
			GetEffectsHanldler getEffects = default,
			GetExecutionHanldler getExecution = default,
			GetCancellationHanldler getCancellation = default
			) {
			Source = source;
			UsersGroup = userGroup;
			TargetsGroup = targetGroup;
			GetEffects = getEffects;
			GetExecution = getExecution;
			GetCancellation = getCancellation;
		}

		public ChainAction GetAction(IEnumerable users, IEnumerable targets) => new(
			Source, users,
			GetEffects?.Invoke(users, targets),
			GetExecution?.Invoke(users, targets),
			GetCancellation?.Invoke(users, targets));
	}

	public class ActionIntention : ActionIntentionSettings {
		public static IEnumerable NotAssigned = new int[0];

		IEnumerable users = NotAssigned;
		IEnumerable targets = NotAssigned;

		public IEnumerable Users { get => users; set { if(UsersGroup.HasChoice) users = value; } }
		public IEnumerable Targets { get => targets; set { if(TargetsGroup.HasChoice) targets = value; } }

		public ActionIntention(ActionIntentionSettings settings) : base(
			settings.Source, settings.UsersGroup, settings.TargetsGroup,
			settings.GetEffects, settings.GetExecution, settings.GetCancellation
			) {
			if(!UsersGroup.HasChoice) users = UsersGroup.Valid;
			if(!TargetsGroup.HasChoice) targets = TargetsGroup.Valid;
		}

		public bool HasUsers() => Users != NotAssigned;
		public bool HasTargets() => Targets != NotAssigned;

		public void ResetUsers() { if(UsersGroup.HasChoice) Users = NotAssigned; }
		public void ResetTargets() { if(TargetsGroup.HasChoice) Targets = NotAssigned; }

		public ChainAction GetAction() {
			if(!HasUsers() || !HasTargets()) Debug.LogError("Users or Targets not Assigned.");
			return GetAction(Users, Targets);
		}
	}
}