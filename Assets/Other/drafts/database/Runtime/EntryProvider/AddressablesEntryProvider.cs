#if UNITY_ADDRESSABLES
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace Drafts.Databases {
	public class AddressablesEntryProvider<T> : Database<T>.DatabaseEntryProvider where T : IDatabaseEntry {
		protected override IEnumerator<IEnumerable<ITEM>> _LoadEntries<ITEM>() {
			if(!typeof(UnityEngine.Object).IsAssignableFrom(typeof(ITEM))) {
				yield return TypeCache<ITEM>.InstantiateAll();
				yield break;
			}

			var locations = Addressables.LoadResourceLocationsAsync(typeof(ITEM).Name, typeof(ITEM));
			while(!locations.IsDone) yield return null;

			var handle = Addressables.LoadAssetsAsync<ITEM>(locations.Result, null);
			while(!handle.IsDone) yield return null;

			Addressables.Release(locations);
			Addressables.Release(handle);

			yield return handle.Result;
		}
	}
}
#endif