using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;

namespace Drafts.Databases {

	public interface IDatabaseEntry : IComparable<IDatabaseEntry>, IComparable<string> {
		string Name { get; }
		int IComparable<string>.CompareTo(string other) => Name.CompareTo(other);
		int IComparable<IDatabaseEntry>.CompareTo(IDatabaseEntry other) => Name.CompareTo(other.Name);
	}

	public partial class Database<T> {

		public interface ISubDatabaseBase {
			IEnumerable<T> All { get; }
			IEnumerator Load();
		}

		public abstract class DatabaseEntryProvider {
			Dictionary<Type, IEnumerator> loading = new();

			public IEnumerator<IEnumerable<ITEM>> LoadEntries<ITEM>() where ITEM : T => loading.TryGetValue(typeof(ITEM), out var ie)
				? (IEnumerator<IEnumerable<ITEM>>)ie : (IEnumerator<IEnumerable<ITEM>>)(loading[typeof(ITEM)] = _LoadEntries<ITEM>());

			protected abstract IEnumerator<IEnumerable<ITEM>> _LoadEntries<ITEM>();

			public IEnumerable<ITEM> LoadAllEntries<ITEM>() where ITEM : T {
				var ie = LoadEntries<ITEM>();
				while(ie.MoveNext()) { }
				return ie.Current;
			}
		}

		public interface ISubDatabase<ITEM> : ISubDatabaseBase, IEnumerable<ITEM> where ITEM : T {
			IEnumerator ISubDatabaseBase.Load() => EntryProvider.LoadEntries<ITEM>();
			DatabaseEntryProvider EntryProvider { get; }

			IEnumerable<ITEM> Filter(Func<ITEM, bool> filter) => All.Where(filter);
			IEnumerable<S> Filter<S>() where S : ITEM => All.Where(a => a is S).Select(a => (S)a);
			IEnumerable<S> Filter<S>(Func<S, bool> filter) where S : ITEM => All.Where(a => a is S s && filter(s)).Select(a => (S)a);

			ITEM Find(string name) {
				if(string.IsNullOrEmpty(name)) return default;
				var item = All.BinaryFindOrDefault(name);
				if(item == null) throw new Exception($"Entry {name} not found in {typeof(ITEM).Name} list.");
				return item;
			}

			protected IReadOnlyList<ITEM> all { get; set; }
			new IReadOnlyList<ITEM> All => all ??= EntryProvider.LoadAllEntries<ITEM>().OrderBy(i => i.Name).ToList();
			IEnumerable<T> ISubDatabaseBase.All => All.Cast<T>();

			IEnumerator<ITEM> IEnumerable<ITEM>.GetEnumerator() => All.GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() => All.GetEnumerator();
		}

	}
}