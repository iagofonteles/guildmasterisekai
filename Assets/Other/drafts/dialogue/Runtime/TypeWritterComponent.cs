using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Drafts.Components {

	/// <summary>Makes the letters of a text appear on by one with a delay.</summary>
	[AddComponentMenu("Drafts/Components/Type Writter")]
	[DisallowMultipleComponent]
	public class TypeWritterComponent : MonoBehaviour {
		public TextMeshProUGUI _text;
		public float delay = 0.0125f;
		public bool Done => _text.maxVisibleCharacters >= _text.GetParsedText().Length;

		public bool pause;
		Timer timer = new Timer();
		public UnityEvent OnTypingComplete;
		public UnityEvent<int> OnShowCharacter;
		public UnityEvent<string> OnTextChanged;

		private void Start() => _text.maxVisibleCharacters = 0;
		private void Update() { if(!pause && timer[delay]) ShowNext(); }

		void ShowNext() {
			_text.maxVisibleCharacters++;
			if(_text.maxVisibleCharacters < _text.GetParsedText().Length)
				OnShowCharacter.Invoke(_text.maxVisibleCharacters - 1);

			if(_text.maxVisibleCharacters == _text.GetParsedText().Length) {
				OnTypingComplete.Invoke();
				enabled = false;
			}
		}

		public string text {
			get => _text.text;
			set {
				enabled = true;
				if(value == null) value = "";
				_text.text = value;
				_text.maxVisibleCharacters = 0;
				OnTextChanged.Invoke(value);
			}
		}

		public void ForceComplete() { _text.maxVisibleCharacters = _text.text.Length; ShowNext(); }

		public void Restart() { text = _text.text; }

	}
}

