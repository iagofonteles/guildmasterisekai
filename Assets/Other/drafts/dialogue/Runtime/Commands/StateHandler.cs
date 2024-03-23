using UnityEngine;
namespace Drafts.Dialogues.Commands {

	public class StateHandler : Command {
		public StateHandler(DialogueController dialog) : base(dialog) { }
		public override string Name => "/state";

		public override string Help { get; } = @"/state operation value
at the end of the dialogue, an event is raised with the current state as argument
you can use it to set individual state of DialogueTrigger instances
operation: set, concat, add, mult
- add and mult handle int _values
- concat concatenates with a coma(,)";

		protected override void Parse() {
			var v = param[2];
			switch(param[1]) {
				case "set": DLG.State = v; break;

				case "concat": DLG.State += v; break;

				case "add":
					DLG.State = float.TryParse(DLG.State, out float i)
						? (i + float.Parse(v)).ToString() : v;
					break;

				case "mult":
					DLG.State = float.TryParse(DLG.State, out float ii)
						? (ii * float.Parse(v)).ToString() : "0";
					break;

				default: Debug.LogError($"{param[1]} is not a valid operation"); break;
			}
		}
	}
}
