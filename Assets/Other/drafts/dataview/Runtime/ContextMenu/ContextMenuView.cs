using Drafts.Extensions;
using TMPro;
using UnityEngine;

namespace Drafts.UI {
	public class ContextMenuView : DataView<ContextMenu> {
		[SerializeField] internal RectTransform container;
		[SerializeField] TextMeshProUGUI title;
		[SerializeField] ContextMenuItemView template;

		protected override void Subscribe() {
			title.TrySetText(Data.title);
			template.Template(Data.itens);
		}
	}
}
