using System;
using System.Collections.Generic;
namespace Drafts.Internationalization {
	public class TableNotFound : Exception {
		public TableNotFound(string lang, string table) : base($"Table '{table}' Lang '{lang}'") { }
	}
	public interface IFileProvider {
		string GetTableText(string lang, string table);
		IEnumerator<string> GetTableTextIE(string lang, string table) { yield return GetTableText(lang, table); }
	}
}
