﻿using UnityEngine.AI;

namespace GuildMasterIsekai {

	public class CostumerVisitorAI : BaseVisitorAI {
		Costumer costumer;
		Guild guild;
		GuildHall hall;

		public CostumerVisitorAI(Costumer costumer, NavMeshAgent agent, HallSpots spots)
			: base(agent, spots) {
			this.costumer = costumer;
			guild = Game.Save.Get<Guild>();
			hall = Game.Save.Get<GuildHall>();
			hall.Costumers.Add(costumer);

			CheckQueue();
		}

		protected override void Destroy() {
			base.Destroy();
			hall.Costumers.Remove(costumer);
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

			Wait(20, () => request.Reply(-1));
		}

		public void Reply(ProblemRequest request, int reply) {
			hall.ProblemRequests.Remove(request);
			Leave();
			if(reply < 0) return;

			foreach(var quest in request.Problem.Outcomes[reply])
				hall.QuestBoard.Add(quest);
		}
	}
}
