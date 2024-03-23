using UnityEngine;
using System;

namespace Drafts.Dialogues {
	public class DialogueController {
		public delegate void CallbackHandler(string state);

		/// <summary>Simply Split the string on end-of-line character(\n).</summary>
		public Func<string, string[]> FileParser = s => s.Split('\n');
		/// <summary>Use it to extract commands from lines.</summary>
		public Func<string, string> LineParser = s => s;

		[SerializeField] protected bool pause;
		[SerializeField] protected bool isOpen;
		/// <summary>Current sentence in sentences.</summary>
		public int currentId;
		/// <summary>All scene sentences not parsed.</summary>
		public string[] sentences;
		/// <summary>Current state. Will be passed to the callback at the end.</summary>
		public string State;
		/// <summary>.</summary>
		CallbackHandler callback;

		public bool IsOpen => isOpen;

		public event Action OnSetup;
		public event Action<string> OnSentenceChanged;
		public event Action<bool> OnPause;
		public event Action OnEnd;

		#region Public 
		/// <summary>Return true if a dialogue has started and not yet ended.</summary>
		/// <summary>Pause flag. Cannot call Next while paused.</summary>
		public bool Pause {
			get => pause;
			set {
				if(value == pause) return;
				pause = value;
				OnPause?.Invoke(value);
			}
		}

		/// <summary>Start a dialogue if there is none.</summary>
		public void Play(string textToParse) => _Play(textToParse);
		/// <summary>Start a dialogue if there is none.</summary>
		public void Play(string textToParse, CallbackHandler callback = null) => _Play(textToParse, callback);
		/// <summary>Start a dialogue if there is none.</summary>
		public void Play(TextAsset asset) => _Play(asset.text);
		/// <summary>Start a dialogue if there is none.</summary>
		public void Play(TextAsset asset, CallbackHandler callback = null) => _Play(asset.text, callback);
		/// <summary>Start a dialogue if there is none.</summary>
		public void Play(DialogueSO so) {
			so.script.initialize?.Invoke();
			_Play(so.script.Text, s => so.script.callback.Invoke(s));
		}
		#endregion

		/// <summary>Start a dialogue if there is none.</summary>
		void _Play(string text, CallbackHandler callback = null) {
			if(isOpen) return;
			isOpen = true;
			State = ""; // reset state
			this.callback = callback;

			ParseSentences(text, true);
			OnSetup?.Invoke();
			Next();
		}

		/// <summary>Call FileParser to recalculate sentences strings.</summary>
		public void ParseSentences(string text, bool resetToFirst) {
			if(resetToFirst) currentId = -1;
			sentences = FileParser(text);
		}

		/// <summary>Calls OnSentenceChange. Begins the next sentence in diaogue script.
		/// calls ExtractCharacterName and ExtractText from processor. (see processor for more info)
		/// If characterName evaluates to null the last valid name is used, to clear it set if to a blank string. ("")
		/// If returned text IsNullOrWhiteSpace it is skipped and next line call imediately.</summary>
		public void Next() {
			if(Pause) return;
			currentId++;

			// no more sentences
			if(currentId >= sentences.Length) { Skip(); return; }

			// skip blank lines
			var sentence = LineParser(sentences[currentId]);
			if(string.IsNullOrWhiteSpace(sentence)) { Next(); return; }

			OnSentenceChanged?.Invoke(sentence);
		}

		/// <summary>Force dialogue to end triggering callbacks. (state may be broken, use with caution)</summary>
		public void Skip() {
			isOpen = false;
			OnEnd?.Invoke();
			callback?.Invoke(State);
		}
	}
}