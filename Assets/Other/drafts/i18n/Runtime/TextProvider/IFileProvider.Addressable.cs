#if DRAFTS_USE_ADDRESSABLES
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Drafts.Internationalization {

	/// <summary>
	/// Uses addressables to load TextAssets.
	/// lang and table strings comes from I18n.Lang and I18n.GetTable.
	/// The path shoud be like in the addressables bundle.
	/// </summary>
	public class AddressablesTableProvider : IFileProvider {

		public string PathFormat { get; }
		public bool LogErrors { get; }

		/// <summary>{0} lang, {1} table. Ex: "i18n/{1}_{0}.txt"</summary>
		public AddressablesTableProvider(string pathFormat, bool logErrors = false) {
			PathFormat = pathFormat;
			LogErrors = logErrors;
		}

		public string GetTableText(string lang, string table) {
			var path = string.Format(PathFormat, lang, table);
			var locHandle = Addressables.LoadResourceLocationsAsync(path);
			var location = locHandle.WaitForCompletion().FirstOrDefault();
			Addressables.Release(locHandle);

			if(location == null) {
				if(LogErrors) Debug.LogError($"<b>Drafts I18N:</b> invalid Addressables path {path}");
				throw new TableNotFound(lang, table);
			}

			var handle = Addressables.LoadAssetAsync<TextAsset>(location);
			var text = handle.WaitForCompletion()?.text;
			Addressables.Release(handle);
			return text;
		}

		public IEnumerator<string> GetTableTextIE(string lang, string table) {
			var path = string.Format(PathFormat, lang, table);
			var locHandle = Addressables.LoadResourceLocationsAsync(path);
			while(!locHandle.IsDone) yield return null;
			var location = locHandle.Result.FirstOrDefault();
			Addressables.Release(locHandle);

			if(location == null) {
				if(LogErrors) Debug.LogError($"*Drafts I18N:* invalid Addressables path {path}");
				throw new TableNotFound(lang, table);
			}

			var hanlde = Addressables.LoadAssetAsync<TextAsset>(location);
			while(!hanlde.IsDone) yield return null;
			var text = hanlde.Result.text;
			Debug.Log(text);
			Addressables.Release(hanlde);

			yield return text;
		}
	}
}
#endif