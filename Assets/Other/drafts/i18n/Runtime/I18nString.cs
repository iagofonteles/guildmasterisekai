using System;
using System.Collections.Generic;
using UnityEngine;
namespace Drafts.Internationalization {

	[Serializable]
	public class I18nString {
		[SerializeField] string table, key;
		I18nEntry entry;
		UnityEngine.Object ctx;

		public I18nString(string table, string key, UnityEngine.Object ctx) {
			this.table = table;
			this.key = key;
			this.ctx = ctx;
		}

		public string Resolve(I18n instance, UnityEngine.Object ctx) {
			try {
				return entry ??= instance.GetTable(table)[key];
			} catch(Exception e) {
				Debug.LogException(e, ctx ?? this.ctx);
				return $"[{key}]";
			}
		}

		public IEnumerator<string> IE(I18n instance, Action<string> callback = null) {
			callback?.Invoke($"[Loading {table}/{key}]");
			if(entry == null) {
				var ie = instance.GetTableIE(table);
				while(ie.MoveNext()) { yield return null; }
				entry = ie.Current[key];
			}
			callback?.Invoke(entry.Text);
			yield return entry.Text;
		}

#if DRAFTS_UTILITY
		public void Async(I18n instance, Action<string> callback = null) => IE(instance, callback).Start();
#endif
	}
}
