using UnityEngine;
using UnityEngine.UI;
using Drafts.Components;
using TMPro;

namespace Drafts.Dialogues {

	public interface IDialogViewModifier {
		void Subscribe(DialogueController controller);
		void Unsubscribe(DialogueController controller);
	}

	/// <summary>If string starts with '@' the character name exists and ends in the firts ':' occurrence.
	/// Removos '@*: ' from the string where * is the extracted character name. (note: it removes one space after the full-colom)
	/// ex: @Nomad Soldier: Hello guys!
	/// CharacterName = "Nomad Soldier"
	/// Text = "Hello Guys!"</summary>
	[DisallowMultipleComponent, RequireComponent(typeof(TypeWritterComponent))]
	public class DialogueView : MonoBehaviour {

		#region Components
		[Header("Components")]
		[Tooltip("Character diplay name")]
		[SerializeField] TextMeshProUGUI nameText;

		[Tooltip("Displays next sentence")]
		public Button nextButton;

		[Tooltip("(optional) Animator with Open, Next and Close states")]
		[SerializeField] Animator animimator;

		[HideInInspector] public TypeWritterComponent typeWritter;
		#endregion

		[ReadOnly] public string characterName;

		DialogueController _controller;

		public DialogueController Controller {
			get => _controller; set {
				if(_controller != null) Unsubscribe();
				_controller = value;
				if(_controller != null) Subscribe();
			}
		}

		void Subscribe() {
			Controller.OnSetup += Setup;
			Controller.OnSentenceChanged += Next;
			Controller.OnEnd += End;
			Controller.OnPause += PauseWritter;

			foreach(var item in GetComponents<IDialogViewModifier>())
				item.Subscribe(_controller);
		}

		void Unsubscribe() {
			Controller.OnSetup -= Setup;
			Controller.OnSentenceChanged -= Next;
			Controller.OnEnd -= End;
			Controller.OnPause -= PauseWritter;

			foreach(var item in GetComponents<IDialogViewModifier>())
				item.Unsubscribe(_controller);
		}

		protected virtual void Awake() {
			if(nextButton) nextButton.onClick.AddListener(TryNext);
			typeWritter = GetComponent<TypeWritterComponent>();
		}

		protected virtual void OnDestroy() {
			if(_controller != null) {
				Unsubscribe();
				_controller.Skip();
			}
		}

		public void TryNext() {
			if(pauseInput) return;
			if(typeWritter.Done) Controller.Next();
			else typeWritter.ForceComplete();
		}

		void PauseWritter(bool b) => typeWritter.pause = b;

		string ExtractCharacterName(string s) {
			if(s[0] != '@') return s;

			var name = s.Substring(1, s.IndexOf(':') - 1);
			nameText.text = name;
			nameText.gameObject.SetActive(!string.IsNullOrWhiteSpace(nameText.text));

			return s.Substring(s.IndexOf(':') + 2); ;
		}

		bool pauseInput;
		public bool PauseInput {
			get => pauseInput;
			set {
				pauseInput= value;
			}
		}


		#region Events
		/// <summary>Reset variables and Play Start animation;</summary>
		protected virtual void Setup() {
			gameObject.SetActive(true);
			characterName = "";
			if(animimator) animimator.Play("Open");
		}

		protected virtual void Next(string sentence) {
			sentence = ExtractCharacterName(sentence);

			if(nextButton) nextButton.Select();
			if(animimator) animimator.Play("Next");
			typeWritter.text = sentence;
		}

		protected virtual void End() {
			if(animimator) animimator.Play("Close");
			else gameObject.SetActive(false);
		}
		#endregion
	}
}
