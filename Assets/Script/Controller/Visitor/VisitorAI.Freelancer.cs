using Drafts;
using UnityEngine;
using UnityEngine.AI;
namespace GuildMasterIsekai {

	public class FreelancerVisitorAI : BaseVisitorAI {
		[SerializeField] Adventurer adventurer;

		Guild guild;
		GuildHall hall;

		public FreelancerVisitorAI(Adventurer adventurer, NavMeshAgent agent, HallSpots spots)
			: base(agent, spots) {
			this.adventurer = adventurer;
			guild = Game.Save.Get<Guild>();
			hall = Game.Save.Get<GuildHall>();
			hall.Freelancers.Add(adventurer);

			CheckActions();
		}

		protected override void Destroy() {
			base.Destroy();
			hall.Freelancers.Remove(adventurer);
		}

		void CheckActions() {
			var spot = hall.QuestBoard.Count > 0 ? Spots.Board.GetFreeSpot() : null;
			if(spot != null) { WalkTo(spot, GetQuest); return; }

			spot ??= Spots.Table.GetFreeSpot();
			if(spot != null) { WalkTo(spot, Rest); return; }

			Leave();
		}

		void GetQuest() {
			Wait(10, () => {
				if(Spots.Counter.IsFull) { Leave(); return; }
				WaitInQueue(Spots.Counter, WaitReply);
			});
		}

		void WaitReply() {
			var quest = hall.QuestBoard.RandomOrDefault();
			var request = new QuestRequest(adventurer, quest);
			request.OnReplied += Reply;
			hall.QuestRequests.Add(request);

			Wait(20, () => request.Reply(null));
		}

		public void Reply(QuestRequest request, Quest quest) {
			hall.QuestRequests.Remove(request);
			if(quest == request.Quest) { Leave(); return; }

			var spot = Spots.Table.GetFreeSpot();
			if(spot != null) { WalkTo(spot, Rest); return; }

			Leave();
		}

		void Rest() => Wait(12, CheckActions);
	}
}
