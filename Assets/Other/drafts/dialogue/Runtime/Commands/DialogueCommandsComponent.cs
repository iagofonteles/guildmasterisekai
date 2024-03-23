using Drafts.Components;
using Drafts.Dialogues;
using Drafts.Dialogues.Commands;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Drafts.Samples {

	/// <summary>
	/// Attach this component to your DialogueUI, it will override the Dialogue.LineParser to parse commands
	/// note: unless specified, parameters cannot have spaces
	/// </summary>
	[DisallowMultipleComponent]
	public class DialogueCommandsComponent : MonoBehaviour, IDialogViewModifier {

		[Tooltip("Character diplay name")]
		[SerializeField] TextMeshProUGUI nameText;
		[Tooltip("The name of the game objects are used in the /image_slot command")]
		public Image[] imageSlots;
		[Tooltip("")]
		public Sprite[] imageDB;
		[Tooltip("Children need a TMProUGUI attached")]
		public Button choiceTemplate;
		[Tooltip("")]
		public Button nextButton;
		public TypeWritterComponent typewritter;

		public RectTransform textBox;
		public Transform textBoxSlotsParent;

		public List<Command> commands = new();

		void IDialogViewModifier.Subscribe(DialogueController controller) {
			controller.LineParser = CommandParser;

			// register commands
			commands.AddRange(new Command[] {
				new End(controller),
				new ActorName(controller, nameText),
				new DebugLog(controller),
				new StateHandler(controller),
				new Label(controller),
				new Jump(controller),
				new Choice(controller, typewritter, nextButton, choiceTemplate, "_"),
				new ImageSlot(controller, imageSlots, imageDB),
				new BoxPosition(controller, textBox, textBoxSlotsParent.GetComponentsImmediate<RectTransform>()),
			});
		}
		void IDialogViewModifier.Unsubscribe(DialogueController controller) { }

		string CommandParser(string text) {
			// find command and call it
			var param = text.Split(' ');
			var command = commands.FirstOrDefault(c => c.Name == param[0]);
			command?.Execute(param);

			// clear text if command was found
			return command == null ? text : null;
		}

		[ContextMenu("Help")]
		public void DebugHelp() {
			Debug.Log(commands.Aggregate("Available commands:", (a, b) => a + "\n\n" + b.Help));
		}
	}
}
