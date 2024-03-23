using TMPro;
namespace Drafts.Dialogues.Commands {
	public class ActorName : Command {
		public ActorName(DialogueController d, TextMeshProUGUI actorName) 
			: base(d) => this.actorName = actorName;
		public override string Name => "/name";

		public override string Help { get; } = @"/name charName
put the specified charName on a separeted text box, charName can have spaces";

		TextMeshProUGUI actorName;
		protected override void Parse() => actorName.text = param[1];
	}
}
