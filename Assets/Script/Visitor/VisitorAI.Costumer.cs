using GuildMasterIsekai.Model;
using GuildMasterIsekai.System;
using GuildMasterIsekai.Visitor.Spot;
using UnityEngine.AI;

namespace GuildMasterIsekai.Visitor {

	public class CostumerVisitorAI : BaseVisitorAI {
		private Costumer costumer;
		private Guild guild;
		private GuildHall hall;
		private ProblemRequest pendingRequest;

		private static VisitorAIConfig.CostumerConf _config;
		private static VisitorAIConfig.CostumerConf Config => _config ??= UnityEngine.Resources.Load<VisitorAIConfig>(nameof(VisitorAIConfig)).Costumer;

		public CostumerVisitorAI(Costumer costumer, NavMeshAgent agent, HallSpots spots)
			: base(agent, spots) {
			this.costumer = costumer;
			guild = Game.Save.Get<Guild>();
			hall = Game.Save.Get<GuildHall>();
			hall.Costumers.Add(costumer);

			CheckQueue();
		}

		private void CheckQueue() {
			if(Spots.Office.IsFull) Leave();
			else WaitInQueue(Spots.Office, WaitReply);
		}

		private void WaitReply() {
			var problem = guild.Generator.NewProblem(costumer);
			var request = new ProblemRequest(costumer, problem);
			pendingRequest = request;
			request.OnReplied += Reply;
			hall.ProblemRequests.Add(request);

			Wait(Config.sitTime, () => request.Reply(-1));
		}

		public void Reply(ProblemRequest request, int reply) {
			if(request != pendingRequest) return;
			ClearRequest();
			if(reply >= 0 && reply < request.Problem.Outcomes.Length)
				foreach(var quest in request.Problem.Outcomes[reply])
					hall.QuestBoard.Add(quest);

			if(Agent) Leave();
		}

		private void ClearRequest() {
			if(pendingRequest == null) return;
			var request = pendingRequest;
			pendingRequest = null;
			request.OnReplied -= Reply;
			hall.ProblemRequests.Remove(request);
		}

		public override void Dispose() {
			ClearRequest();
			hall.Costumers.Remove(costumer);
			base.Dispose();
		}
	}
}
