using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Drafts.CommadnTerminal {

	public class Path {
		const BindingFlags BINDING = BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance;

		internal Dictionary<string, object> paths = new();
		public IReadOnlyDictionary<string, object> Paths => paths;

		public Path() { }
		public Path(object obj) {
			var cmds = obj.GetType().GetMethods(BINDING);
			var pths = obj.GetType().GetProperties(BINDING).Where(p => p.GetMethod.ReturnType.IsClass);
			foreach(var c in cmds) paths.Add(CamelCase(c.Name), new Command(obj, c));
			foreach(var p in pths) paths.Add(CamelCase(p.Name), new Path(p.GetValue(obj)));
		}

		public static string CamelCase(string s) => char.ToLower(s[0]) + s.Substring(1).Replace('_', ' ');

		public bool HasPath(string path) => paths.ContainsKey(path);
		public bool GetPath(string str, out object val) => paths.TryGetValue(str, out val);

		public IEnumerable<string> LocalPaths(string root) => paths.Select(p => $"{root}{p.Key}");
		public IEnumerable<string> GetAllPaths(string root) => paths.SelectMany(p =>
			p.Value is Path path ? path.GetAllPaths($"{root}{p.Key} ") : new[] { $"{root}{p.Key}" });
	}
}
