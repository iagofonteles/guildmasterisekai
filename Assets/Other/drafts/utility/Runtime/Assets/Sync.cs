#if DRAFTS_ADDRESSABLES
using UnityEngine.AddressableAssets;
using UnityEngine;

namespace Magicards {
	public static class Sync {
		public static T Get<T>(string path) => Addressables.LoadAssetAsync<T>(path).WaitForCompletion();
		public static GameObject Instantiate(string path)
			=> Addressables.InstantiateAsync(path).WaitForCompletion();
		public static T Instantiate<T>(string path) where T : Component
			=> Addressables.InstantiateAsync(path).WaitForCompletion().GetComponent<T>();
	}
}
#endif