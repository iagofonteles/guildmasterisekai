using Drafts.Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Drafts.Dialogues {

	//Show stuff of the class on the inspector
	[CreateAssetMenu(menuName = "Drafts/Dialogue Script")]
	[System.Serializable]
	public class DialogueSO : ScriptableObject {
		public DialogueScript script;
		public static implicit operator DialogueScript(DialogueSO d) => d.script;
		public static implicit operator DialogueSO(DialogueScript d) => new DialogueSO() { script = d };
	}

	//Show stuff of the class on the inspector
	[System.Serializable]
	public class DialogueScript {
		/// <summary>Identifier for search purpose.</summary>
		public string key;
		public UnityEvent initialize;
		public UnityEvent<string> callback = new();

		/// <summary>Sentences of the dialog, will be processed by ExtractCharacterName and ExtractText</summary>
		[SerializeField] TextAsset textAsset;
		[TextArea(1, 100), SerializeField] string text;

		public DialogueScript(string text, DialogueController.CallbackHandler callback = null) {
			this.text = text;
			this.callback.AddListener(callback.Invoke);
		}

		public string Text => textAsset ? textAsset.text : text;
	}
}