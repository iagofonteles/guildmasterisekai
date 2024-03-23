#if DRAFTS_UTILITY
using UnityEngine;
namespace Drafts.Components {
	public class TriggerDraftsEvent : TriggerBase {
		public DraftsEvent onTrigger;
		[ContextMenu("Trigger")]
		public override void Trigger() => onTrigger.Invoke();
	}
}
#endif