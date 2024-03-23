using System;
using System.Collections;
using System.Collections.Generic;

namespace Drafts {
	public interface ISearchSettings {
		string Title { get; }
		IEnumerable GetItens();
		string GetName(object obj);
	}

	public interface ISearchSettings<T>: ISearchSettings {
		new IEnumerable<T> GetItens();
		string GetName(T obj);

		IEnumerable ISearchSettings.GetItens() => GetItens();
		string ISearchSettings.GetName(object obj) {
			if(obj is not T t) throw new Exception("obj is not " + nameof(T));
			return GetName(t);
		}
	}

	public abstract class SearchSettings : ISearchSettings {
		public string Title { get; protected set; }
		protected Func<IEnumerable> GetItens { get; set; }
		protected Func<object, string> GetName { get; set; } = o => 0.ToString();
		IEnumerable ISearchSettings.GetItens() => GetItens();
		string ISearchSettings.GetName(object obj) => GetName(obj);
	}
}