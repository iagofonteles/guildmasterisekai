namespace Drafts.Dialogues.Commands {

	public class ShakeScreen : Command {
		public ShakeScreen(DialogueController dialog) : base(dialog) { }
		public override string Name => "/shake";

		public override string Help { get; } = @"[not implemented] /shake";

		protected override void Parse() {
			// /shake vertical 80
			//switch(param[1]) {
			//	case "vertical": Startcaouritne(ieneime(Int(1))); break;
			//	case "horizontal": Startcaouritne(ieneime(param[2])); break;
			//}
		}
	}
}
