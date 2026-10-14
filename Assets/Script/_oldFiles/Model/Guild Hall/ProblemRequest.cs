using System.IO;
using Drafts.SaveData;
using System;

namespace GuildMasterIsekai {
	[Serializable]
	public class ProblemRequest : IBinarySave {
		public Costumer Costumer { get; private set; }
		public Problem Problem { get; private set; }
		public event Action<ProblemRequest, int> OnReplied;
		public void Reply(int reply) => OnReplied?.Invoke(this, reply);

		public ProblemRequest(Costumer costumer, Problem problem) {
			Costumer = costumer;
			Problem = problem;
		}

		public ProblemRequest() { }

		public void Save(BinaryWriter writer) {
			writer.Write(Costumer != null);
			Costumer?.Save(writer);
			writer.Write(Problem != null);
			Problem?.Save(writer);
		}

		public void Load(BinaryReader reader) {
			Costumer = reader.ReadBoolean() ? new Costumer() : null;
			Costumer?.Load(reader);
			Problem = reader.ReadBoolean() ? new Problem() : null;
			Problem?.Load(reader);
		}
	}
}
