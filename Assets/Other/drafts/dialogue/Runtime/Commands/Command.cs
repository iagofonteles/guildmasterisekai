namespace Drafts.Dialogues {
	public abstract class Command {
		public abstract string Name { get; }
		public abstract string Help { get; }
		protected DialogueController DLG { get; }
		protected string[] param;
		protected int paramInt(int paramId) => int.Parse(param[paramId]);
		protected abstract void Parse();
		public Command(DialogueController dialog) => DLG = dialog;
		public void Execute(string[] str) {
			param = str;
			Parse();
		}
	}
}
