using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Drafts.Databases {
	public class ResourcesEntryProvider<T> : Database<T>.DatabaseEntryProvider where T : IDatabaseEntry {
		string Path { get; }
		/// <summary>Path may contain {0} wich will be repaced by Type.Name.</summary>
		public ResourcesEntryProvider(string path) => Path = path ?? throw new System.ArgumentNullException(nameof(path));

		protected override IEnumerator<IEnumerable<ITEM>> _LoadEntries<ITEM>() {
			if(!typeof(Object).IsAssignableFrom(typeof(ITEM))) {
				yield return TypeCache<ITEM>.InstantiateAll();
				yield break;
			}

			var res = Resources.LoadAll(string.Format(Path, typeof(ITEM).Name), typeof(ITEM));
			yield return res.Cast<ITEM>();
		}
	}
}
