namespace Drafts.Dialogues.Commands {
	public class Label : Command {
		public Label(DialogueController dialog) : base(dialog) { }
		public override string Name => "/label";

		public override string Help { get; } = @"/label label_name
Save a point in dialogue to jump to. Does nothing by itself.";

		protected override void Parse() { }
	}
}
