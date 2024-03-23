using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Drafts.Dialogues {
	public abstract class DialogueTrigger : MonoBehaviour {
		protected abstract DialogueController GetController();

		[Tooltip("TriggerDialogue will play the scene that key matches state, setting state to NullOrEmptyspace via property does not change the current value, so you dont need to set state to current state in every dialog")]
		[SerializeField] string state;
		[Tooltip("Remeber to set the key for each dialogue, so it can be called acording to state")]
		public DialogueScript[] dialogues;
		[Tooltip("")]
		public UnityEvent<string> OnStateChanged;

		[ContextMenu("Trigger")]
		public void TriggerDialogue() => TriggerDialogue(state);

		public void TriggerDialogue(string key) {
			var d = dialogues.FirstOrDefault(d => d.key == key);
			if(d != null) {
				d.initialize?.Invoke();
				GetController().Play(d.Text,
				s => { State = s; d.callback.Invoke(s); });
			}
		}

		/// <summary>Ignore null, empty and blank strings.</summary>
		public string State {
			get => state;
			set {
				if(string.IsNullOrWhiteSpace(value)) return;
				state = value;
				OnStateChanged.Invoke(value);
			}
		}

		/// <summary>Set state to empty string.</summary>
		public void ResetState() => State = "";
	}
}
