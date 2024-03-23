using System.Collections.Generic;

namespace Drafts.Internationalization {
	public class I18nEntry : IComparer<I18nEntry> {
		public static readonly I18nEntry Empty = new(null, "");
		public static Comparer<I18nEntry, string> KeyComparer = new(a => a.Key);
		public I18nEntry(string key, string text) { Key = key; Text = text; }
		public I18nEntry(string text) { Key = null; Text = text; }
		internal virtual string Key { get; }
		public string Text { get; internal set; }
		public static implicit operator string(I18nEntry e) => e.Text ?? $"[{e.Key}]";
		public int Compare(I18nEntry x, I18nEntry y) => x.Key.CompareTo(y.Key);
	}
}
