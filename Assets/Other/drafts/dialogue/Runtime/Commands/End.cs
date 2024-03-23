namespace Drafts.Dialogues.Commands {
	public class End : Command {
		public override string Name => "/end";
		public override string Help { get; } = @"/end [final state] text
Ends the dialogue.
If [final state] is specified, set state before ending the dialogue.";

		public End(DialogueController d) : base(d) { }
		protected override void Parse() {
			if(param.Length > 0) DLG.State = param[1];
			DLG.currentId = DLG.sentences.Length;
		}
	}
}
