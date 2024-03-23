using System.Collections.Generic;
using System.Collections;

namespace Drafts.Internationalization {

	public class EntryNotFound : System.Exception {
		public EntryNotFound(string table, string key) : base($"Table '{table}' Entry '{key}'") { }
	}

	public class I18nTable : IEnumerable<I18nEntry> {

		public I18n I18n { get; }
		public string Name { get; }
		List<I18nEntry> Entries { get; } = new();

		public I18nTable(I18n i18n, string name) {
			I18n = i18n;
			Name = name;
		}

		internal void UpdateEntries(string text) {
			var lines = I18n.FileReader.GetLines(text);

			foreach(var line in lines) {
				var entry = I18n.FileReader.GetEntry(line);
				if(entry == null) continue;

				var id = Entries.BinarySearch(entry, entry);
				if(id < 0) Entries.Insert(~id, entry);
				else Entries[id].Text = entry.Text;
			}
		}

		public I18nEntry this[string key] {
			get {
				var id = Entries.BinarySearch(key, I18nEntry.KeyComparer);
				if(id < 0) {
					id = ~id;
					Entries.Insert(id, new(key, $"[{key}]"));
					throw new EntryNotFound(Name, key);
				}
				return Entries[id];
			}
		}

		public IEnumerator<I18nEntry> GetEnumerator() => Entries.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => Entries.GetEnumerator();
	}
}
