using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace Drafts {
	/// <summary> Uses string.Replace, so its best to add some simbols at the start en end of match strings. ex: {Name}</summary>
	public class FormatSettings<T> {
		Dictionary<string, Func<T, string>> map;
		public FormatSettings(Dictionary<string, Func<T, string>> map) => this.map = map;
		public string Format(string str, T obj, string prefix = "") {
			foreach(var item in map)
				str = Regex.Replace(str, prefix + item.Key, item.Value(obj));
			return str;
		}
	}
}