namespace Drafts {
	public static partial class ExtensionsString {

		public static string TryFormat(this string s, params object[] values) {
			try { return string.Format(s, values); } catch { return s; }
		}

	}
}