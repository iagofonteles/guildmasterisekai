using System;
using System.Collections.Generic;
namespace Drafts.Extensions {
	public static class Dictionary {

		public static T Get<T>(this Dictionary<object, object> dic, object key)
			=> dic.TryGetValue(key, out var v) ? (T)v : default;

		public static T Get<T>(this Dictionary<object, object> dic, object key, T fallback)
			=> dic.TryGetValue(key, out var v) ? (T)v : fallback;

		public static T Get<T>(this Dictionary<object, object> dic, object key, Func<T> fallback)
			=> dic.TryGetValue(key, out var v) ? (T)v : fallback();
	}
}
