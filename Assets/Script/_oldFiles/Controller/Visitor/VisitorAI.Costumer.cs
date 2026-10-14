using Drafts;
using UnityEngine.AI;

namespace GuildMasterIsekai {

	public class CostumerVisitorAI : BaseVisitorAI {
		Costumer costumer;
		Guild guild;
		GuildHall hall;

		static VisitorAIConfig.CostumerConf _config;
		static VisitorAIConfig.CostumerConf Config => _config ??= ResourceUtil.Load<VisitorAIConfig>().Costumer;

		public CostumerVisitorAI(Costumer costumer, NavMeshAgent agent, HallSpots spots)
			: base(agent, spots) {
			this.costumer = costumer;
			guild = Game.Save.Get<Guild>();
			hall = Game.Save.Get<GuildHall>();
			hall.Costumers.Add(costumer);

			CheckQueue();
		}

		void CheckQueue() {
			if(Spots.Office.IsFull) Leave();
			else WaitInQueue(Spots.Office, WaitReply);
		}

		void WaitReply() {
			var problem = guild.Generator.NewProblem(costumer);
			var request = new ProblemRequest(costumer, problem);
			request.OnReplied += Reply;
			hall.ProblemRequests.Add(request);

			Wait(Config.sitTime, Leave);
		}

		public void Reply(ProblemRequest request, int reply) {
			hall.ProblemRequests.Remove(request);
			if(reply >= 0)
				foreach(var quest in request.Problem.Outcomes[reply])
					hall.QuestBoard.Add(quest);

			if(Agent) Leave();
		}
	}
}
