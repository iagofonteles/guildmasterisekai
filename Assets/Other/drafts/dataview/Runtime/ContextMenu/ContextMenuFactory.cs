using UnityEngine;

namespace Drafts.UI {
	public abstract class ContextMenuFactory : MonoBehaviour {
		[SerializeField] ContextMenuView template;

		public void Open(Drafts.DataHolder data) {
			var menu = template.Template(CreateMenu(data.Data));
			if(menu.container) menu.container.position = data.GetComponent<RectTransform>().position;
			ModifyMenu(menu);
		}
		protected abstract ContextMenu CreateMenu(object target);
		protected virtual void ModifyMenu(ContextMenuView view) { }
	}
}
