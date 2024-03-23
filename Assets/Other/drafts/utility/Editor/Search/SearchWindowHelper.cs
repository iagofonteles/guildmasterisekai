using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
namespace DraftsEditor {
	public static class SearchWindowHelper {

		public static void OpenWindow<T>(this T provider, VisualElement anchor = null) where T : ScriptableObject, ISearchWindowProvider {
			var pos = new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition));
			if(anchor != null) {
				var point = anchor.worldBound.center + new Vector2(0, anchor.worldBound.height + 8);
				pos = new SearchWindowContext(GUIUtility.GUIToScreenPoint(point));
			}
			SearchWindow.Open(pos, provider);
		}

		public static void OpenWindow(this SearchProvider provider, Action<object> onSelected, VisualElement anchor = null) {
			var pos = new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition));
			if(anchor != null) {
				var point = anchor.worldBound.center + new Vector2(0, anchor.worldBound.height + 8);
				pos = new SearchWindowContext(GUIUtility.GUIToScreenPoint(point));
			}
			provider.onSelected = onSelected;
			SearchWindow.Open(pos, provider);
		}

		public static SearchTreeEntry AddEntry(this List<SearchTreeEntry> list, string label, int level, object data = null) {
			var entry = new SearchTreeEntry(new GUIContent(label));
			entry.level = level;
			entry.userData = data;
			list.Add(entry);
			return entry;
		}

		public static SearchTreeGroupEntry AddGroup(this List<SearchTreeEntry> list, string label, int level, object data = null) {
			var entry = new SearchTreeGroupEntry(new GUIContent(label));
			entry.level = level;
			entry.userData = data;
			list.Add(entry);
			return entry;
		}

		public static void CloseSearchWindow() {
			try {
				var field = typeof(SearchWindow).GetField("s_FilterWindow", BindingFlags.NonPublic | BindingFlags.Static);
				var window = (SearchWindow)field.GetValue(null);
				window?.Close();
			} catch { }
		}
	}
}
