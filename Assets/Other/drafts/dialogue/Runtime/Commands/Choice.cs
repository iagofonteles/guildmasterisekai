using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Drafts.Components;

namespace Drafts.Dialogues.Commands {

	public class Choice : Command {
		public override string Name => "/choice";

		public override string Help { get; } = @"/choice label_to_jump text
if this choice is selected, dialogue will jump to specified label
everithing before label is text and can have spaces";

		int choice = -1;
		List<string> choiceLabels = new List<string>();
		Button nextButton;
		Button choiceTemplate;
		List<Button> buttons = new();
		GameObject parent;
		string continueFlag;
		TypeWritterComponent typewritter;

		public Choice(DialogueController d, TypeWritterComponent typewritter, Button nextButton, Button choiceTemplate, string continueFlag = "-") : base(d) {
			parent = choiceTemplate.transform.parent.gameObject;
			this.choiceTemplate = choiceTemplate;
			this.typewritter = typewritter;
			this.continueFlag = continueFlag;
			this.nextButton = nextButton;
			choiceTemplate.gameObject.SetActive(false);
			Reset();
		}

		Button NewButton() {
			var b = Object.Instantiate(choiceTemplate, parent.transform);
			var id = buttons.Count;
			b.onClick.AddListener(() => choice = id);
			return b;
		}

		protected override void Parse() {
			// save label
			choiceLabels.Add(param[1]);
			if(buttons.Count < choiceLabels.Count)
				buttons.Add(NewButton());

			// set button text
			var btn = buttons[choiceLabels.Count - 1];
			btn.gameObject.SetActive(true);
			var text = string.Join(" ", param.Skip(2));
			foreach(var t in btn.GetComponentsInChildren<TextMeshProUGUI>(true))
				t.text = text;

			if(DLG.sentences[DLG.currentId + 1].StartsWith("/choice")) {
				DLG.currentId++;
				Execute(DLG.sentences[DLG.currentId].Split(' '));
			} else
				WaitChoice().Start();
		}

		IEnumerator WaitChoice() {
			parent.SetActive(true);
			yield return null;

			buttons[0].Select();
			// disable next button and wait
			nextButton.interactable = false;
			DLG.Pause = true;
			typewritter.ForceComplete();
			yield return new WaitWhile(() => choice < 0);
			DLG.Pause = false;
			nextButton.interactable = true;

			// jump and reset UI
			if(choiceLabels[choice] != continueFlag)
				Jump.JumpTo(DLG, choiceLabels[choice]);
			Reset();
			DLG.Next();
		}

		void Reset() {
			parent.SetActive(false);
			choiceLabels.Clear();
			choice = -1;
			foreach(var b in buttons)
				b.gameObject.SetActive(false);
		}
	}
}
