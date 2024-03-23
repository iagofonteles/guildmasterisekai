#if DRAFTS_DATAVIEW
using UnityEngine;
using UnityEngine.Events;

namespace Drafts {
	public class IntWatcherView : DataView<Watcher<int>> {
		[SerializeField] FormattedText value;
		[SerializeField] UnityEvent<int> onValueChanged;
		[SerializeField] string clearText = "-";
		protected override void Subscribe() => Data.OnChanged.AddListener(OnChanged);
		protected override void Unsubscribe() => Data.OnChanged.RemoveListener(OnChanged);
		private void OnChanged(int v) => Refresh();
		protected override void Repaint() {
			value.TrySetValue(Data.Value);
			onValueChanged.Invoke(Data.Value);
		}
		public override void Clear() => value.TrySetValue(clearText);
	}
}
#endif