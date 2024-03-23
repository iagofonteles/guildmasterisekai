using UnityEngine;
namespace Drafts {

	public interface ISingleLineInspector {
		float RightPadding => 0;
		(string, string, float)[] Configs => null;
	}

	public interface ISinglePropertyInspector {
		string Name => null;
	}

	public interface IInspectorTooltip {
		string Tooltip { get; }
	}

	public class SingleLineInspector : PropertyAttribute {
		public float RightPadding { get; }
		public string[] Fields { get; }

		public SingleLineInspector(params string[] fields) {
			Fields = fields;
		}

		public SingleLineInspector(float rightPadding, params string[] fields) {
			Fields = fields;
			RightPadding = rightPadding;
		}
	}
}
