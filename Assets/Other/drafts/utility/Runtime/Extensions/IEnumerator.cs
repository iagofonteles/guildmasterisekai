using System;
using System.Collections;
using System.Collections.Generic;

namespace Drafts.Extensions {
	public static class ExtensionsIEnumerator {

		[Obsolete]
		public static object GetResult(this IEnumerator ie) {
			while(ie.MoveNext()) { }
			return ie.Current;
		}

		[Obsolete]
		public static T GetResult<T>(this IEnumerator<T> ie) {
			while(ie.MoveNext()) { }
			return ie.Current;
		}

	}
}
