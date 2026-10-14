using System.IO;
using Drafts.SaveData;
using System;

namespace GuildMasterIsekai {
	[Serializable]
	public class QuestRequest : IBinarySave {
		public Adventurer Adventurer { get; private set; }
		public Quest Quest { get; private set; }
		public event Action<QuestRequest, Quest> OnReplied;
		public void Reply(Quest reply) => OnReplied?.Invoke(this, reply);

		public QuestRequest(Adventurer adventurer, Quest quest) {
			Adventurer = adventurer;
			Quest = quest;
		}

		public QuestRequest() { }

		public void Save(BinaryWriter writer) {
			writer.Write(Adventurer != null);
			Adventurer?.Save(writer);
			writer.Write(Quest != null);
			Quest?.Save(writer);
		}

		public void Load(BinaryReader reader) {
			Adventurer = reader.ReadBoolean() ? new Adventurer() : null;
			Adventurer?.Load(reader);
			Quest = reader.ReadBoolean() ? new Quest() : null;
			Quest?.Load(reader);
		}
	}
}
