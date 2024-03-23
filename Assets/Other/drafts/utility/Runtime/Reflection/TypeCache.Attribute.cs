using System;
namespace Drafts {

	public class TypeNameSearchSettings : ISearchSettings {
		CachedDerivedTypes cache;
		public string Title { get; }
		public System.Collections.IEnumerable GetItens() => cache.Names;
		public string GetName(object obj) => obj as string;
		public TypeNameSearchSettings(Type type) {
			Title = "Subtypes: " + type.Name;
			cache = TypeCache.GetCache(type);
		}
	}

	public class TypeNameAttribute : SearchAttribute {
		public TypeNameAttribute(Type type) : base(new TypeNameSearchSettings(type), true) { }
	}
}