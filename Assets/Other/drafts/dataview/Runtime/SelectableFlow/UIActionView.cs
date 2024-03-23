#if ENABLE_INPUT_SYSTEM
using Drafts.Extensions;
using TMPro;
using UnityEngine;
namespace Drafts.UI.Flow {

	public class UIActionView : DataView<UIAction> {
		[SerializeField] TextMeshProUGUI text;
		[SerializeField] CanvasGroup canvas;
		[SerializeField] float altAlpha = .4f;
		protected override void Repaint() {
			text.TrySetText(Data.Name);
			canvas.TrySetAlpha(Data.IsValid ? 1 : altAlpha);
		}
	}
}
#endif