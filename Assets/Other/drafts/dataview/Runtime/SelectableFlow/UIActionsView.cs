#if ENABLE_INPUT_SYSTEM
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Drafts.UI.Flow {

	public class UIActionsView : DataView<IEnumerable<UIAction>> {
		[SerializeField] Transform actionsParent;
		List<UIActionView> children;

		private void Awake() {
			children = actionsParent.gameObject.GetComponentsImmediate<UIActionView>();
			UIAction.OnChanged += SetData;
		}
		protected override void OnDestroy() {
			base.OnDestroy();
			UIAction.OnChanged -= SetData;
		}

		void SetData() => Data = UIAction.Current;

		protected override void Repaint() {
			foreach(var view in children) {
				view.Data = UIAction.All.FirstOrDefault(a => a.Control == view.name);
				view.gameObject.SetActive(view.Data != null);
			}
		}

		public override void Clear() {
			foreach(var view in children) {
				view.Data = null;
				view.gameObject.SetActive(false);
			}
		}
	}
}
#endif