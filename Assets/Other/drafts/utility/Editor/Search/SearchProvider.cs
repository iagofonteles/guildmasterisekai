using Drafts;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace DraftsEditor {

	public class SearchProvider : ScriptableObject, ISearchWindowProvider {

		static SearchProvider() => AssetSearchSettings._findAssets = (t, f) => EditorUtil.FindAssets(t, f);

		internal ISearchSettings settings;
		public Action<object> onSelected;

		public static SearchProvider Create(ISearchSettings settings, Action<object> onSelected) {
			var so = CreateInstance<SearchProvider>();
			so.settings = settings;
			so.onSelected = onSelected;
			return so;
		}

		public static SearchProvider Create<T>(ISearchSettings settings, Action<T> onSelected) {
			var so = CreateInstance<SearchProvider>();
			so.settings = settings;
			so.onSelected = obj => onSelected((T)obj);
			return so;
		}

		//public void Open(VisualElement anchor = null) => this.OpenWindow(anchor);

		public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context) {
			var list = new List<SearchTreeEntry>();
			list.AddGroup(settings.Title, 0);

			foreach(var asset in settings.GetItens())
				list.AddEntry(settings.GetName(asset), 1, asset);

			//if(list.Count == 2) {
			//	onSelected(list[1].userData);
			//	SearchWindowHelper.CloseSearchWindow();
			//	return list;
			//}
			return list;
		}

		public virtual bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context) {
			onSelected(entry.userData);
			return true;
		}

		public static implicit operator SearchProvider(SearchSettings settings) => Create(settings, null);
	}

	public static class ExtensionsISearchSettings {
		public static void Search(this ISearchSettings settings, Action<object> onSelected)
			=> SearchProvider.Create(settings, onSelected).OpenWindow();
		public static void Search<T>(this ISearchSettings<T> settings, Action<T> onSelected)
			=> SearchProvider.Create(settings, onSelected).OpenWindow();
	}
}