using TMPro;
using UnityEngine;
namespace Drafts.Components {

	[ExecuteAlways, RequireComponent(typeof(TextMeshProUGUI), typeof(RectTransform))]
	public class RelativeFontSize : MonoBehaviour {

		[Range(.01f, 1.5f)]
		public float multiplier = 1;
		public bool useWidth;
		TextMeshProUGUI text;
		RectTransform rect;

		private void Start() {
			text = GetComponent<TextMeshProUGUI>();
			rect = GetComponent<RectTransform>();
		}

		private void OnValidate() => OnRectTransformDimensionsChange();

		private void OnRectTransformDimensionsChange() {
			if(!text) return;
			var v = useWidth ? rect.rect.width : rect.rect.height;
			text.fontSize = v * multiplier;
		}

		[ContextMenu("Set From Current Size")]
		public void SetFromCurrentSize() {
			var v = useWidth ? rect.rect.width : rect.rect.height;
			multiplier = text.fontSize / v;
		}
	}
}
