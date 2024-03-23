using UnityEngine.Events;

namespace Drafts.UI {
	public class DataViewExtras : DataView {
		public UnityEvent OnNotNull;
		public UnityEvent OnNull;
		protected override void Subscribe() => OnNotNull.Invoke();
		protected override void Unsubscribe() => OnNull.Invoke();
	}
}
