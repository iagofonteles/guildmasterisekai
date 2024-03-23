using Drafts.Components;
using Drafts.Patterns;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Drafts.Samples {
	[DisallowMultipleComponent, RequireComponent(typeof(TypeWritterComponent))]
	public class TypeWritterCommandsComponent : MonoBehaviour {

		public char commandDelimiter = '&';
		[HideInInspector] public TypeWritterComponent typeWritter;

		public float[] speeds;

		List<TypeWritterCommands.Command> commands = new List<TypeWritterCommands.Command>();

		public void Start() {
			typeWritter = GetComponent<TypeWritterComponent>();
			typeWritter.OnTypingComplete.AddListener(ClearCommands);
			typeWritter.OnShowCharacter.AddListener(ParseCommand);

			commands.AddRange(new[]{
				new TypeWritterCommands.Speed(typeWritter, speeds),
			});
		}

		void ClearCommands() {
			var txt = typeWritter._text.text;
			var rgx = new Regex($"{commandDelimiter}.+?{commandDelimiter}"); // remove commands
			txt = rgx.Replace(txt, "");
			txt = txt.Replace("&&", "&");
			typeWritter._text.text = txt;
		}

		void ParseCommand(int pos) {
			var text = typeWritter._text.text;
			if(text[pos] == commandDelimiter) {
				var cmd = text.Substring(pos + 1);
				cmd = cmd.Substring(0, cmd.IndexOf(commandDelimiter)); // get full comand
				text = text.Remove(pos, cmd == "" ? 1 : cmd.Length + 2); // if double delimiter, return delimiter
				typeWritter._text.text = text; // return text without the command

				if(cmd != "") {
					var param = cmd.Split(' ');
					var c = commands.FirstOrDefault(c => c.Name == param[0]);
					if(c != null) c.Execute(param);
				}
			}
		}
	}
}
