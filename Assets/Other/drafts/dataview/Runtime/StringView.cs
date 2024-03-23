using Drafts;
using TMPro;
using UnityEngine;
public class StringView : DataView<string> {
	[SerializeField] TextMeshProUGUI text;
	protected override void Repaint() => text.text = Data;
	public override void Clear() => text.text = "";
}
