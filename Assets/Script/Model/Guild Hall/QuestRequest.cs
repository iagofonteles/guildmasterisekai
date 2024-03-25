using System;

namespace GuildMasterIsekai {
	[Serializable]
	public class QuestRequest {
		public Adventurer Adventurer { get; }
		public Quest Quest { get; }
		public event Action<QuestRequest, Quest> OnReplied;
		public void Reply(Quest reply) => OnReplied?.Invoke(this, reply);

		public QuestRequest(Adventurer adventurer, Quest quest) {
			Adventurer = adventurer;
			Quest = quest;
		}
	}
}
