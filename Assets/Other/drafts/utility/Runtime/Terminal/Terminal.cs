using System;
using System.Collections.Generic;
using System.Linq;

namespace Drafts.CommadnTerminal {

	public enum Result { Close, Keep, Error }

	public class Terminal {

		public Path Root { get; } = new();

		public Terminal() {
			Root.paths.Add("clear", GetType().GetMethod("Clear"));
			Root.paths.Add("help", GetType().GetMethod("help"));
		}

		public string Output { get; private set; } = "";
		public event Action<string> OnOutput;
		public void Out(string s) { Output += "\n" + s; OnOutput?.Invoke(s); }

		public void AddCommands(string path, object obj) => Root.paths.Add(Path.CamelCase(path), new Path(obj));

		public Result Execute(string command) {
			if(string.IsNullOrWhiteSpace(command)) return Result.Close;
			command = command.Trim();
			var path = Root;
			var args = new Queue<string>(command.Split(' '));

			while(args.Count > 0) {
				var curr = args.Dequeue();
				if(!path.paths.TryGetValue(curr, out var val)) {
					Out("Invalid command: " + curr);
					break;
				}
				if(val is Command c) {
					Out(command);
					var (r, s) = c.Execute(args);
					if(!string.IsNullOrWhiteSpace(s)) Out(s);
					return r;
				}
				if(val is Path p) { path = p; continue; }
				Out("Invalid object in paths: " + val);
				break;
			}
			return Result.Error;
		}

		public IEnumerable<string> GetAllPaths() => Root.GetAllPaths("");
		public void Clear() { Output = ""; Out(""); }
		public void Help() { Out(GetAllPaths().Aggregate("\n", (a, b) => $"{a}\n{b}")); Out(""); }
	}
}
