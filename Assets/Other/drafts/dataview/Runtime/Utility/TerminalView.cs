#if DRAFTS_UTILITY
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Drafts.CommadnTerminal.UI {

	public class TerminalView : MonoBehaviour {

		[SerializeField] TMP_InputField inputField;
		[SerializeField] TextMeshProUGUI history;
		[SerializeField] TextMeshProUGUI sugestion;

		Terminal terminal;
		Path currPath;

		public Terminal Terminal {
			get => terminal;
			set {
				if(terminal != null) terminal.OnOutput -= OnOutput;
				terminal = value;
				if(terminal != null) terminal.OnOutput += OnOutput;
			}
		}

		private void OnEnable() => inputField.SelectDelayed();
		private void OnDestroy() {
			if(terminal != null) terminal.OnOutput -= OnOutput;
		}

		public void Clear() => terminal.Clear();

		public void AutoCompleteWord() {
			if(sugestion.text.Contains('(')) return;
			var sugests = sugestion.text.Split('\n');
			if(sugests.Count() == 1) {
				var text = sugests.First() + " ";
				inputField.text = text;
				inputField.stringPosition = text.Length;
			}
		}

		private void OnOutput(string obj) => history.text = terminal.Output;

		public void OnInputChanged(string input) {
			if(string.IsNullOrWhiteSpace(input)) {
				sugestion.text = "";
				return;
			}
			string track = "", last = "";
			var words = input.Split(' ');
			currPath = Terminal.Root;
			Command cmdFound = null;
			foreach(var w in words) {
				last = w;
				if(!currPath.GetPath(w, out var v)) break;
				if(v is Command c) cmdFound = c;
				if(v is Path p) currPath = p;
				track += w + " ";
			}
			if(cmdFound != null) {
				sugestion.text = track + cmdFound.Signature;
				return;
			}
			var pttrn = track + last.Aggregate("", (a, b) => a + b + ".*?");
			var paths = currPath.LocalPaths(track);
			var matches = paths.Where(s => Regex.IsMatch(s, pttrn, RegexOptions.IgnoreCase));
			sugestion.text = string.Join('\n', matches);
		}

		public void Submit() {
			var result = terminal.Execute(inputField.text);
			if(result != Result.Error) inputField.text = "";
			inputField.SelectDelayed();
			if(result == Result.Close) gameObject.SetActive(false);
			sugestion.text = "";
		}
	}
}
#endif