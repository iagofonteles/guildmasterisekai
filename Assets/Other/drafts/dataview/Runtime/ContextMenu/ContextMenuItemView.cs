using Drafts.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Drafts.UI {

	public class ContextMenuItemView : DataView<ContextMenu.Item> {
		[SerializeField] ContextMenuView parent;
		[SerializeField] Button button;
		[SerializeField] Image icon;
		[SerializeField] TextMeshProUGUI text;
		[SerializeField] bool hideIfInvalid;

		private void Awake() => button.onClick.AddListener(Execute);

		protected override void Subscribe() {
			icon.TrySetSprite(Data.icon);
			icon.TrySetActive(Data.icon);
			text.TrySetText(Data.text);
			button.interactable = Data.valid;
			gameObject.SetActive(!hideIfInvalid || button.interactable);
		}

		void Execute() {
			var res = Data.action();
			switch(res) {
				case bool b: if(!b) Destroy(parent); break;
				case ContextMenu menu: parent.Data = menu; break;
				default: Destroy(parent.gameObject); break;
			}
		}
	}
}
