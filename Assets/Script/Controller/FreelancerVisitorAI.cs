using Drafts;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace GuildMasterIsekai {
	public class FreelancerVisitorAI : IVisitorAI {
		Adventurer adventurer;
		NavMeshAgent agent;
		GuildSpots spots;

		float restTime;
		bool replied;

		Guild guild;
		GuildHall hall;

		Action nextAction;
		Func<bool> meta;

		public FreelancerVisitorAI(Adventurer adventurer, NavMeshAgent agent, GuildSpots spots) {
			this.adventurer = adventurer;
			this.agent = agent;
			this.spots = spots;
			guild = Game.Save.Get<Guild>();
			hall = Game.Save.Get<GuildHall>();

			meta = () => true;
			nextAction = WalkToBoard;
		}

		public bool DestinationReached => meta();

		bool ReachDestination() => agent.remainingDistance < 1;
		bool GetReply() => replied;
		bool WaitTillRested() => (restTime -= Time.deltaTime) <= 0;

		public void Act() => nextAction();

		void WalkToBoard() {
			agent.SetDestination(spots.board.position);
			meta = ReachDestination;
			nextAction = GetQuest;
		}

		void GetQuest() {
			agent.SetDestination(spots.counter.position);
			meta = ReachDestination;
			nextAction = WaitReply;
		}

		void WaitReply() {
			var quest = guild.QuestBoard.RandomOrDefault();
			var request = new QuestRequest(adventurer, quest);
			request.OnReplied += Reply;
			hall.Requests.Add(request);

			replied = false;
			meta = GetReply;
			nextAction = null;
		}

		public void Reply(QuestRequest request, Quest quest) {
			replied = true;
			hall.Requests.Remove(request);
			nextAction = quest == request.Quest ? Leave : Rest;
		}

		void Leave() {
			agent.SetDestination(spots.exit.position);
			meta = ReachDestination;
			nextAction = Destroy;
		}

		void Rest() {
			agent.SetDestination(spots.table.position);
			restTime = 30;
			meta = WaitTillRested;
			nextAction = CheckRemainingTime;
		}

		void CheckRemainingTime() {
			Leave();
		}

		void Destroy() {
			UnityEngine.Object.Destroy(agent.gameObject);
		}
	}
}

