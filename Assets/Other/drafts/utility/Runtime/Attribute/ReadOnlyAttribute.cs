using UnityEngine;

namespace Drafts {
	public class ReadOnlyAttribute : PropertyAttribute {
		bool runtime, editor;
		public bool Enable => Application.isPlaying ? runtime : editor;

		public ReadOnlyAttribute(bool editor = false, bool runtime = false) {
			this.editor = editor;
			this.runtime = runtime;
		}

	}
}
