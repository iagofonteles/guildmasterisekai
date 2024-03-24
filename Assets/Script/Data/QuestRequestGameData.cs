using Drafts;

namespace GuildMasterIsekai {
	public class QuestRequestGameData : DataView<QuestRequest> {
		void Start() => GetComponent<IData>().Data = Game.Save.Get<GuildHall>().Requests;
	}
}