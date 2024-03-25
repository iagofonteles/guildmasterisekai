using System;

namespace GuildMasterIsekai {
	[Serializable]
	public class ProblemRequest {
		public Costumer Costumer { get; }
		public Problem Problem { get; }
		public event Action<ProblemRequest, int> OnReplied;
		public void Reply(int reply) => OnReplied?.Invoke(this, reply);

		public ProblemRequest(Costumer costumer, Problem problem) {
			Costumer = costumer;
			Problem = problem;
		}
	}
}
