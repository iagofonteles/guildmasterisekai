#if DRAFTS_USE_ADDRESSABLES
using System.Collections;
using UnityEngine.AddressableAssets;

namespace Drafts {

	public class AddressableSearchSettings<T> : SearchSettings {
		string title, label;

		string ISearchSettings.Title => title;
		IEnumerable ISearchSettings.GetItens() => Addressables.LoadAssetsAsync<T>(label, null).WaitForCompletion();
		string ISearchSettings.GetName(object obj) => (obj as UnityEngine.Object).name;

		public AddressableSearchSettings(string label) {
			title = $"{typeof(T).Name} in {label}";
			this.label = label ?? typeof(T).Name;
		}
	}
}
#endif