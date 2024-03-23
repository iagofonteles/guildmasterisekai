using UnityEngine;

namespace Drafts {
	/// <summary>Only works with a serialized bool field in the same script.</summary>
	public class ConditionalAttribute : PropertyAttribute {
		public string field;
		public bool inverse;
		public float value;
		public string str;

		public ConditionalAttribute() { }

		public ConditionalAttribute(string field, bool inverse = false) {
			this.field = field.Replace("[", ".Array.data[");
			this.inverse = inverse;
		}

		public ConditionalAttribute(string field, float value, bool inverse = false) {
			this.field = field.Replace("[", ".Array.data[");
			this.value = value;
			this.inverse = inverse;
		}

		public ConditionalAttribute(string field, string str, bool inverse = false) {
			this.field = field.Replace("[", ".Array.data[");
			this.str = str;
			this.inverse = inverse;
		}

		public bool Validate(bool v) => v ^ inverse;
		public bool Validate(int v) => v == (int)value ^ inverse;
		public bool Validate(float v) => v == value ^ inverse;
		public bool Validate(string v) => v == str ^ inverse;
		public bool Validate(Object v) => v ^ inverse;
	}
}
