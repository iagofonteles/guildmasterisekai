using System.Linq;
using UnityEngine;
namespace Drafts.Dialogues.Commands {
	public class DebugLog : Command {
		public DebugLog(DialogueController dialog) : base(dialog) { }
		public override string Name => "/debug";

		public override string Help { get; } = @"/debug text
simple log something in the editor, text can have scpaces";

		protected override void Parse() => Debug.Log(string.Join(" ", param.Skip(1)));
	}
}
