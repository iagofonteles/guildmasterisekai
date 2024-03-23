using UnityEngine;
namespace Drafts {
	public abstract class SearchAttribute : PropertyAttribute {
		public ISearchSettings Settings { get; }
		public bool Lock { get; }
		public SearchAttribute(ISearchSettings settings) => Settings = settings;
		public SearchAttribute(ISearchSettings settings, bool @lock) {
			Settings = settings;
			Lock = @lock;
		}
	}
}