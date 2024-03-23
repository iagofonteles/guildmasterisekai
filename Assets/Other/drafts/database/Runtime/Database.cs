using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;

namespace Drafts.Databases {

	/// <summary>The database stores references to commonly used resources statically for easy access.</summary>
	public partial class Database<T> : IEnumerable<Database<T>.ISubDatabaseBase> where T : IDatabaseEntry {
		DatabaseEntryProvider EntryProvider { get; }

		public Database(DatabaseEntryProvider entryProvider)
			=> EntryProvider = entryProvider ?? throw new ArgumentNullException(nameof(entryProvider));

		Dictionary<Type, ISubDatabaseBase> databases = new();
		public IEnumerable<T> GetEverything() => databases.SelectMany(p => p.Value.All);
		public IEnumerator<ISubDatabaseBase> GetEnumerator() => databases.Select(p => p.Value).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public ITEM Find<ITEM>(string name) where ITEM : T => GetSubBase<ITEM>().Find(name);
		public IReadOnlyList<ITEM> All<ITEM>() where ITEM : T => GetSubBase<ITEM>().All;
		public IEnumerable<ITEM> Filter<ITEM>(Func<ITEM, bool> filter) where ITEM : T => GetSubBase<ITEM>().Filter(filter);
		public IEnumerable<ITEM> Filter<BASE, ITEM>() where BASE : T where ITEM : BASE => GetSubBase<BASE>().Filter<ITEM>();
		public IEnumerable<ITEM> Filter<BASE, ITEM>(Func<ITEM, bool> filter) where BASE : T where ITEM : BASE => GetSubBase<BASE>().Filter(filter);

		public IEnumerator Load<ITEM>() where ITEM : T => GetSubBase<ITEM>().Load();
		public IEnumerator LoadAll() {
			var loads = databases.Select(b => b.Value.Load()).ToArray();
			foreach(var item in loads) yield return item;
		}

		public ISubDatabase<ITEM> GetSubBase<ITEM>() where ITEM : T => databases.TryGetValue(typeof(ITEM), out var db)
			? (ISubDatabase<ITEM>)db : (ISubDatabase<ITEM>)(databases[typeof(ITEM)] = new SubDatabase<ITEM>(EntryProvider));

		public class SubDatabase<ITEM> : ISubDatabase<ITEM> where ITEM : T {
			IReadOnlyList<ITEM> ISubDatabase<ITEM>.all { get; set; }
			public DatabaseEntryProvider EntryProvider { get; }

			public SubDatabase(DatabaseEntryProvider entryProvider)
				=> EntryProvider = entryProvider;
		}
	}
}
