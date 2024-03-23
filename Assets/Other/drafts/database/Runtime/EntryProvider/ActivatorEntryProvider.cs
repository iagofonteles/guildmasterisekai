using System.Collections.Generic;
namespace Drafts.Databases {
	/// <summary>Uses Activator with TypeCache to create instances.</summary>
	public class ActivatorEntryProvider<T> : Database<T>.DatabaseEntryProvider where T : IDatabaseEntry {
		protected override IEnumerator<IEnumerable<ITEM>> _LoadEntries<ITEM>() {
			yield return TypeCache<ITEM>.InstantiateAll();
		}
	}
}
