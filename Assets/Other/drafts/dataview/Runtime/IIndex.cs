namespace Drafts {

	public interface IIndex {
		int Index { get; set; }
	}

	public static class ExtensionsIIndex {
		public static void SetIndex(this IIndex i, int index) { i.Index = index; }
	}

}
