#if DRAFTS_ADDRESSABLES
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Drafts {

	[Obsolete]
	public static class DefaultAsset {
		static Dictionary<Type, object> map = new();
		public static void Set<T>(T value) => map[typeof(T)] = value;
		public static T Get<T>() => map.TryGetValue(typeof(T), out var obj)
			? (T)obj : throw new Exception("No default value for " + typeof(T).Name);
	}

	public class Async {
		public static IEnumerable<string> AllLoadedStrings(string key) {
			var resOperation = Addressables.LoadResourceLocationsAsync(key);
			return resOperation.WaitForCompletion().Select(l => l.PrimaryKey);
		}
	}

	public class Async<T> where T : UnityEngine.Object {
		readonly string path, fallback;
		public T Value { get; private set; }
		readonly UnityEngine.Object contetx;
		AsyncOperationHandle<IList<IResourceLocation>>? resOperation;
		AsyncOperationHandle<T>? operation;

		public void UseAsync(Action<T> action) => UseIE(action).Start();

		public IEnumerator UseIE(Action<T> callback = null) {
			if(Value) {
				callback?.Invoke(Value);
				yield break;
			}

			if(!resOperation.HasValue) {
				resOperation = Addressables.LoadResourceLocationsAsync(path, typeof(T));
				resOperation.Value.Completed += ResCompleted;
			}
			yield return resOperation.Value;

			if(resOperation.Value.Status == AsyncOperationStatus.Failed || !resOperation.Value.Result.Any()) {
				resOperation = Addressables.LoadResourceLocationsAsync(fallback, typeof(T));
				resOperation.Value.Completed += ResCompleted;
			}
			yield return resOperation.Value;

			if(resOperation.Value.Status == AsyncOperationStatus.Failed || !resOperation.Value.Result.Any()) {
				Debug.LogError($"Failed to load {path} and fallback {fallback}", contetx);
				callback?.Invoke(null);
				yield break;
			}

			if(!operation.HasValue) {
				try {
					operation = Addressables.LoadAssetAsync<T>(resOperation.Value.Result.First());
				} catch(InvalidOperationException e) {
					Debug.LogError($"Path: {path}, Fallback: {fallback}", contetx);
					Debug.LogException(e, contetx);
					callback?.Invoke(null);
					yield break;
				}
				operation.Value.Completed += op => Value = op.Result;
			}
			yield return operation.Value;

			callback?.Invoke(Value);
			yield return Value;
		}

		public IEnumerator Wait => UseIE(null);

		private void ResCompleted(AsyncOperationHandle<IList<IResourceLocation>> locs) {
			if(locs.Status == AsyncOperationStatus.Failed || locs.Result.Count == 0)
				Debug.LogWarning("Resource failed to load: " + path);
#if !UNITY_EDITOR
			else if(locs.Result.Count > 1)
				Debug.LogWarning("Duplicate addressable key: " + path);
#endif
		}

		//public Async() { }
		public Async(T value) => this.Value = value;
		public Async(string path, string fallback, UnityEngine.Object ctx) {
			this.path = path;
			this.fallback = fallback;
			contetx = ctx;
		}
		public static implicit operator bool(Async<T> a) => a != null && a.Value;
		//public static implicit operator T(Async<T> a) => a.value;
		//public static implicit operator Async<T>(string path) => new(path);

	}

	public static class ExtensionsAsync {
		public static void InstantiateAsync(this Async<GameObject> a, Transform parent, Action<GameObject> callback = null)
			=> InstantiateAsync(a, Vector3.zero, Quaternion.identity, parent, callback);

		public static void InstantiateAsync(this Async<GameObject> a, Vector3 position, Quaternion? rotation = null, Transform parent = null, Action<GameObject> callback = null) {
			void OnLoad(GameObject item) {
				var obj = item ? UnityEngine.Object.Instantiate(item, position, rotation ?? Quaternion.identity, parent) : null;
				callback?.Invoke(obj);
			}
			a.UseAsync(OnLoad);
		}

		public static void InstantiateAsync<T>(this Async<GameObject> a, Transform parent, Action<T> callback = null)
			where T : Component => InstantiateAsync(a, Vector3.zero, Quaternion.identity, parent, callback);

		public static void InstantiateAsync<T>(this Async<GameObject> a, Vector3 position, Quaternion? rotation = null, Transform parent = null, Action<T> callback = null)
			where T : Component {
			void OnLoad(GameObject item) {
				var obj = item ? UnityEngine.Object.Instantiate(item, position, rotation ?? Quaternion.identity, parent) : null;
				callback?.Invoke(obj?.GetComponent<T>());
			}
			a.UseAsync(OnLoad);
		}
	}
}
#endif