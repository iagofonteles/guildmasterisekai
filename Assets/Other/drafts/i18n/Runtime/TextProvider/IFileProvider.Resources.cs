#if DRAFTS_USE_ADDRESSABLES
using System.Collections.Generic;
using UnityEngine;

namespace Drafts.Internationalization {

	/// <summary>
	/// Uses Resources folder to load TextAssets.
	/// lang and table strings comes from I18n.Lang and I18n.GetTable
	/// The path shoud be like: i18n/table_lang.txt
	/// </summary>
	public class ResourcesTextProvider : IFileProvider {
		string PathFormat { get; }

		/// <summary>{0} lang, {1} table. Ex: "i18n/{1}_{0}"</summary>
		public ResourcesTextProvider(string pathFormat) => PathFormat = pathFormat;

		public string GetTableText(string lang, string table) {
			var path = string.Format(PathFormat, lang, table);
			var text = Resources.Load<TextAsset>(path)?.text;
			if(text == null) throw new TableNotFound(lang, table);
				//Debug.LogError($"*Drafts I18N:* invalid Resources path {path}");
			return text;
		}

		public IEnumerator<string> GetTableTextIE(string lang, string table) { yield return GetTableText(lang, table); }
	}
}
#endif