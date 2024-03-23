using System;
using TMPro;
using UnityEngine;

namespace Drafts {
	[Serializable]
	public class FormattedText {
		[SerializeField] TMP_Text text;
		string baseText;

		public string Text => text.text;
		public Color Color { get => text.color; set => text.color = value; }

		public void SetValue(object value) {
			baseText ??= text.text.Contains('{') ? text.text : "{0}";
			text.text = string.Format(baseText, value);
		}

		public void SetValue(params object[] values) {
			baseText ??= text.text.Contains('{') ? text.text : "{0}";
			text.text = string.Format(baseText, values);
		}

		public static implicit operator bool(FormattedText v) => v != null && v.text;
	}

	public static class ExtensionsFormattedTextView {
		public static void TrySetValue(this FormattedText v, object value) { if(v) v.SetValue(value); }
		public static void TrySetValue(this FormattedText v, params object[] values) { if(v) v.SetValue(values); }
		public static void TrySetColor(this FormattedText v, Color value) { if(v) v.Color = value; }
	}
}
