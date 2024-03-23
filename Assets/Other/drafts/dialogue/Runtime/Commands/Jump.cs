using UnityEngine;
using System;
namespace Drafts.Dialogues.Commands {

	public class Jump : Command {
		public Jump(DialogueController dialog) : base(dialog) { }
		public override string Name => "/jump";

		public override string Help { get; } = @"/jump label_name
jump to specified label. Use it for branching acording to player answers";

		protected override void Parse() => JumpTo(DLG, param[1]);

		public static void JumpTo(DialogueController ctrl, string label) {
			var id = Array.IndexOf(ctrl.sentences, $"/label {label}");
			if(id < 0) Debug.LogError($"Dialogue: label {label} not found");
			ctrl.currentId = id - 1;
		}
	}
}
