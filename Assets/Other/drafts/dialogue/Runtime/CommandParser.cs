using System;
using System.Collections.Generic;
using System.Linq;

namespace Drafts.Dialogues {
	/// <summary>parse strings into methods with ease.</summary>
	[Obsolete]
	public class CommandParser {
		Type type;
		object instance = null;
		public string Error { get; private set; }
		public object Result { get; private set; }
		public Dictionary<Type, Converter> converter;

		public object TargetInstance { get => instance; set { type = value.GetType(); instance = value; } }
		public Type TargetType { get => type; set { type = value; instance = null; } }

		public delegate bool Converter(string s, out object o);
		static bool convS(string s, out object o) { o = s; return true; }
		static bool convI(string s, out object o) { var b = int.TryParse(s, out var v); o = v; return b; }
		static bool convF(string s, out object o) { var b = float.TryParse(s, out var v); o = v; return b; }
		static bool convI2(string s, out object o) { var b = int.TryParse(s, out var v); o = v; return b; }
		static bool convF2(string s, out object o) { var b = int.TryParse(s, out var v); o = v; return b; }

		public CommandParser(object instance) : this(instance.GetType()) { this.instance = instance; }
		public CommandParser(Type type) {
			this.type = type;
			converter = new Dictionary<Type, Converter>() {
				//{ typeof(string), convS },
				{ typeof(string), convS },
				{ typeof(int), convI },
				{ typeof(float), convF },
				{ typeof(int[]), convI2 },
				{ typeof(float[]), convF2 }
			};
		}

		public bool Parse(string command) {
			Error = "";
			var args = command.Split(' ');

			var method = type.GetMethod(args[0]);
			if(method == null) {
				var member = type.GetMember(args[0]);
				if(member == null) {
					Error = "Member [" + args[0] + "] does not exist.";
					return false;
				}
				Error = "Method [" + args[0] + "] does not exist.";
				return false;
			}

			var param = method.GetParameters();
			var values = new object[param.Length];

			// wrong number of arguments
			if(!param.Any(p => p.GetCustomAttributes(typeof(ParamArrayAttribute), false).Length > 0))
				if(param.Length != args.Length - 1) {
					Error = "Wrong number of parameters.";
					goto GenerateError;
				}

			for(int i = 0; i < param.Length; i++) {

				// parse arguments
				var t = param[i].ParameterType;
				if(converter.TryGetValue(t, out var func))
					if(func(args[i + 1], out var v))
						values[i] = v;
					else
						Error += $"Parameter {i + 1} needs to be {t.Name}.\n";
			}
			if(Error == "") {
				Result = method.Invoke(instance, values);
				return true;
			}

		// generate help string
		GenerateError:
			Error += param.Aggregate(string.Format("\n{0}",method.Name),
				(s, p) => string.Format("{0} {1}({2})", s, p.Name, p.ParameterType.Name));
			var help = type.GetProperty(args[0] + "_help");
			if(help != null)
				Error += "\n" + (string)help.GetValue(instance); // help text if any
			return false;
		}

		public bool Parse(string command, out object result) {
			var b = Parse(command);
			result = Result;
			return b;
		}

	}
}
