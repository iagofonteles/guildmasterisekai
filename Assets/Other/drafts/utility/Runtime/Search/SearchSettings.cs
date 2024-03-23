using System;
using System.Collections.Generic;
using System.Linq;
namespace Drafts {
	public class SearchSettings<T> : SearchSettings {
		public SearchSettings(Func<IEnumerable<T>> getItens, Func<T, string> getName = null) {
			GetItens = getItens;
			if(getName != null) GetName = o => getName((T)o);
		}
	}
}