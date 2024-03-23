using System;
using System.Collections;
using System.Collections.Generic;

namespace Drafts.Internationalization {
	public class I18n {
		public bool LogErrors { get; }
		public string Lang { get; private set; }
		public IFileProvider TableProvider { get; }
		public IFileReader FileReader { get; }
		public event Action<string> OnLanguageChanged;
		Dictionary<string, I18nTable> tables = new();

		public I18n(string defLang, IFileProvider tableProvider, IFileReader fileReader, bool logErrors = false) {
			Lang = defLang;
			TableProvider = tableProvider;
			FileReader = fileReader;
			LogErrors = logErrors;
		}

		public I18nEntry this[string table, string key] => GetTable(table)[key];

		public I18nTable GetTable(string table) {
			if(!tables.TryGetValue(table, out var tab)) {
				tab = new I18nTable(this, table);
				tables.Add(table, tab);
				var text = TableProvider.GetTableText(Lang, table);
				tab.UpdateEntries(text);
			}
			return tab;
		}

		public IEnumerator<I18nTable> GetTableIE(string table) {
			if(!tables.TryGetValue(table, out var tab)) {
				tab = new I18nTable(this, table);
				tables.Add(table, tab); // create table
				var ie = TableProvider.GetTableTextIE(Lang, table);
				while(ie.MoveNext()) { yield return null; }
				var text = ie.Current; // update text
				tab.UpdateEntries(text);
			}
			yield return tab;
		}

		/// <summary>lang is used by the TextProvider to load files.</summary>
		public IEnumerator SetLanguageIE(string lang) {
			if(Lang == lang) yield break;
			Lang = lang;
			foreach(var table in tables) {
				var ie = TableProvider.GetTableTextIE(Lang, table.Value.Name);
				yield return ie;
				var text = ie.Current; // update text
				table.Value.UpdateEntries(text);
			}
			OnLanguageChanged?.Invoke(lang);
		}
	}
}
