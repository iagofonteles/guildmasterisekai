using System;

namespace Drafts {

	/// <summary>Use this to sync a UI element with some data structure using events from the data structure.
	/// Can also be used to avoid destroiyng UI elements and instead.
	/// Auto Subscribe Repaint to IOnChanged on Awake.</summary>
	/// <typeparam name="DATA">The data structure to sync with.</typeparam>
	public abstract class DataView : DataHolder, IDataView {
		[NonSerialized] bool repaintOnEnable;
		public event Action onDestroy;

		protected virtual void OnEnable() {
			if(!repaintOnEnable) return;
			repaintOnEnable = false;
			CallRepaintOrClear();
		}

		protected virtual void OnDestroy() {
			if(data != null) Unsubscribe();
			onDestroy?.Invoke();
		}

		protected override void SetData(object value) {
			if(data != null) Unsubscribe();
			data = value is IData d ? d.Data : value;
			if(data != null) Subscribe();
			OnDataChanged.Invoke(value);
			Refresh();
		}

		/// <summary>default: Refresh on IOnChanged or IOnModified<>.</summary>
		protected virtual void Subscribe() { }
		/// <summary>Unsubscribe to DATA events and hide UI.</summary>
		protected virtual void Unsubscribe() { }
		/// <summary>Called on Data changed if enabled, or the next time it is enabled.</summary>
		protected virtual void Repaint() { }
		/// <summary>Called when DATA == null.</summary>
		public virtual void Clear() { }

		/// <summary>Repaint if its enabled or the next time it is enabled.</summary>
		public void Refresh() {
			if(isActiveAndEnabled) CallRepaintOrClear();
			else repaintOnEnable = true;
		}

		void CallRepaintOrClear() { if(Data != null) Repaint(); else Clear(); }
	}
}
