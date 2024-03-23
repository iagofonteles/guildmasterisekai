#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UObj = UnityEngine.Object;
using UnityEditor;
using System.IO;

namespace DraftsEditor {
	public static partial class EditorUtil {

		public static T GetPreloadedAsset<T>() where T : UObj => (T)PlayerSettings.GetPreloadedAssets().FirstOrDefault(a => a is T);
		public static IEnumerable<T> GetPreloadedAssets<T>() where T : UObj => PlayerSettings.GetPreloadedAssets().Where(a => a is T).Cast<T>();

		public static T FindAssetByGUID<T>(string guid) where T : UObj => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
		public static UObj FindAssetByGUID(string guid, Type type) => AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), type);

		public static IEnumerable<T> FindAssets<T>(string folder = "Assets") where T : UObj
			=> AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { folder }).Select(guid => FindAssetByGUID<T>(guid));

		public static IEnumerable<UObj> FindAssets(Type type, string folder = "Assets") 
			=> AssetDatabase.FindAssets($"t:{type.Name}", new[] { folder }).Select(s => FindAssetByGUID(s, type));

		//public static IEnumerable<T> FindAssets<T>(Predicate<T> predicate, string folder = "Assets") where T : UObj
		//	=> AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { folder }).Select(s => FindAssetByGUID<T>(s)).Where(a => predicate(a));

		//public static IEnumerable<UObj> FindAssets(Type type, Predicate<UObj> predicate, string folder = "Assets")
		//	=> AssetDatabase.FindAssets($"t:{type.Name}", new[] { folder }).Select(s => FindAssetByGUID(s, type)).Where(a => predicate(a));

		public static IEnumerable<T> FindPrefabs<T>(string folder = "Assets", Predicate<Component> predicate = null) where T : Component
			=> FindAssets<GameObject>(folder).Select(g => g.GetComponent<T>()).Where(c => c && (predicate?.Invoke(c) ?? true));

		public static IEnumerable<Component> FindPrefabs(Type type, string folder = "Assets", Predicate<Component> predicate = null)
			=> FindAssets<GameObject>(folder).Select(g => g.GetComponent(type)).Where(c => c && (predicate?.Invoke(c) ?? true));

		/// <summary>Load all assets of T in a subfolder and put in a list.</summary>
		public static List<T> StoreAssetsInList<T>(this UObj obj, ref List<T> list, string folder = "") where T : UObj
			=> list = new List<T>(FindAssets<T>(GetPath(obj, folder)));

		public static string GetPath(this UObj asset, string path = "") {
			if(!asset) return path;
			var p = Path.GetDirectoryName(AssetDatabase.GetAssetPath(asset));
			return Path.Combine(p, path);
		}

	}
}
#endif