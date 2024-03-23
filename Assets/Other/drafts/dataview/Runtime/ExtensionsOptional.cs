using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Drafts.Extensions {
	/// <summary>optional serializable fields for views.</summary>
	public static class ExtensionOptionalFields {

		public static void TrySetText(this Text text, string value) { if(text) text.text = value; }
		public static void TrySetText(this Text text, object value) { if(text) text.text = value.ToString(); }
		public static void TrySetText(this TextMeshProUGUI text, string value) { if(text) text.text = value; }
		public static void TrySetText(this TextMeshProUGUI text, object value) { if(text) text.text = value.ToString(); }
		public static void TrySetColor(this Graphic graphic, Color value) { if(graphic) graphic.color = value; }
		
		public static void TrySetText(this TMP_InputField input, string value) { if(input) input.SetTextWithoutNotify(value); }
		public static void TrySubscribe(this TMP_InputField input, UnityAction<string> value) { if(input) input.onValueChanged.AddListener(value); }
		public static void TryUnsubscribe(this TMP_InputField input, UnityAction<string> value) { if(input) input.onValueChanged.RemoveListener(value); }
		
		public static void TrySetValue(this Slider slider, float value) { if(slider) slider.value = value; }
		public static void TrySetInteractable(this Selectable selectable, bool value) { if(selectable) selectable.interactable = value; }
		public static void TrySetSprite(this Image image, Sprite value) { if(image) { image.sprite = value; image.enabled = value; } }
		public static void TrySetSprite(this SpriteRenderer rend, Sprite value) { if(rend) rend.sprite = value; }
		
		public static void TryEnable(this Behaviour behaviour, bool enabled) { if(behaviour) behaviour.enabled = enabled; }
		public static void TrySetActive(this GameObject go, bool active) { if(go) go.SetActive(active); }
		public static void TrySetActive(this Component c, bool active) { if(c) c.gameObject.SetActive(active); }

		public static void TrySetAlpha(this CanvasGroup c, float value) { if(c) c.alpha = value; }
		public static void TrySetInteractable(this CanvasGroup c, bool value) { if(c) c.interactable = value; }
		public static void TrySetBlocksRaycasts(this CanvasGroup c, bool value) { if(c) c.blocksRaycasts = value; }

		public static void TryPlay(this Animator anim, string stateName, int layer = 0) { if(anim && anim.isActiveAndEnabled) anim.Play(stateName, layer); }
	}
}