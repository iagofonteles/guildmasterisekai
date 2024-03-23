#if DRAFTS_ADDRESSABLES
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Drafts {
	public class AsyncSingleton<T> where T : Component {
		static string key = "singleton/" + typeof(T).FullName;
		static T _instance;
		public static T Instance = _instance ??= Addressables.InstantiateAsync(key).WaitForCompletion().GetComponent<T>();
		public static bool IsInstantiated = _instance;
	}
}
#endif