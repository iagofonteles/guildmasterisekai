using UnityEngine;

public class InlineAttribute : PropertyAttribute {
	public InlineAttribute(params float[] widths) : this(false, widths) { }
	public InlineAttribute(bool showName, params float[] widths) { }
}
