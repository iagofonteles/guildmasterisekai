using System.Collections;
using UnityEngine;

#if EDITOR_COROUTINE
using Unity.EditorCoroutines.Editor;
#endif

namespace Drafts {

#if EDITOR_COROUTINE
	public class HybridCoroutine {
		MonoBehaviour owner;
		Coroutine coroutine;
		EditorCoroutine eCoroutine;

		public HybridCoroutine(MonoBehaviour owner, IEnumerator routine) {
			this.owner = owner;
			//if(Application.isPlaying) coroutine = owner.StartCoroutine(routine);
			//else EditorCoroutineUtility.StartCoroutine(routine, owner);
			while(routine.MoveNext()) { }
		}

		public void Stop() {
			if(coroutine != null) owner.StopCoroutine(coroutine);
			if(eCoroutine != null) EditorCoroutineUtility.StopCoroutine(eCoroutine);
		}
	}
#endif

	public static partial class DraftsUtil {

		public static GameObject InstantiatePrefabHybrid(GameObject prefab, Transform parent)
			=> InstantiatePrefabHybrid(prefab, parent.transform.position, parent.rotation, parent);

		/// <summary>In editor, create prefab instance, else, just duplicate</summary>
		public static GameObject InstantiatePrefabHybrid(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent) {
#if UNITY_EDITOR
			if(Application.isPlaying) return Object.Instantiate(prefab, position, rotation, parent);
			else {
				var go = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab, parent);
				go.transform.position = position;
				go.transform.rotation = rotation;
				return go;
			}
#else
			return Object.Instantiate(prefab, position, rotation, parent);
#endif
		}

		public static void DestroyHybrid(Object obj) {
#if UNITY_EDITOR
			if(Application.isPlaying) Object.Destroy(obj);
			else Object.DestroyImmediate(obj);
#else
			Object.Destroy(obj);
#endif
		}

		public static void AddToAsset(Object obj, Object asset, bool save = true) {
#if UNITY_EDITOR
			UnityEditor.AssetDatabase.AddObjectToAsset(obj, asset);
			if(save) UnityEditor.AssetDatabase.SaveAssets();
#endif
		}

		public static void RemoveFromAsset(Object obj, bool save = true) {
#if UNITY_EDITOR
			UnityEditor.AssetDatabase.RemoveObjectFromAsset(obj);
			if(save) UnityEditor.AssetDatabase.SaveAssets();
#endif
		}
	}
}