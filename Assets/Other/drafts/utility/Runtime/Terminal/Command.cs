using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Drafts.CommadnTerminal {

	public class Command {
		public object Source;
		public MethodInfo Info;
		public string Signature { get; }

		static Dictionary<Type, Func<string, (bool, object)>> castMap { get; } = new() {
			{ typeof(string), s => (true, s) },
			{ typeof(int), s => int.TryParse(s, out var v) ? (true, v) : (false, 0) },
			{ typeof(float), s => float.TryParse(s, out var v) ? (true, v) : (false, 0f) },
			{ typeof(Color), s => ColorUtility.TryParseHtmlString(s, out var v) ? (true, v) : (false, Color.white) },
		};

		public Command(object source, MethodInfo info) {
			Source = source;
			Info = info;
			Signature = string.Join(',', info.GetParameters().Select(p => p.ParameterType.Name));
			Signature = $"({Signature})";
		}

		public (Result, string) Execute(Queue<string> args) {
			// count parameters
			var param = Info.GetParameters();
			if(param.Length > args.Count)
				return (Result.Error, $"-- Parameter count mismatch. Required: {param.Length}.");

			// cast parameters
			object[] casted = new object[param.Length];
			for(int i = 0; i < param.Length; i++) {
				var t = param[i].ParameterType;
				if(!castMap.TryGetValue(t, out var c)) return (Result.Error, $"-- Parameter of type {t.Name} not suported.");
				var (r, v) = c(args.Dequeue());
				if(!r) return (Result.Error, $"-- Parameter {i} must be {t.Name}.");
				casted[i] = v;
			}

			// execute method
			try {
				var ret = Info.Invoke(Source, casted);
				return ret is string s ? (Result.Keep, s) : (Result.Close, null);
			} catch(Exception e) {
				Debug.LogException(e);
				return (Result.Error, "-- Exception: " + e.InnerException.Message);
			}
		}
	}
}
