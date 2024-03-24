using Drafts;
using System;
using UnityEngine;

namespace GuildMasterIsekai {

	[Serializable]
	public class GuildSpots {
		public Transform entrance;
		public Transform exit;
		public Transform counter;
		public Transform board;
		public Transform table;
	}

	public class VisitorController : MonoBehaviour {
		[SerializeField] TimerComponent timer;
		[SerializeField] GuildSpots spots;
		[SerializeField, Prefab] VisitorAI visitorAiPrefab;

		Guild guild;
		GuildHall hall;

		void Start() {
			guild = Game.Save.Get<Guild>();
			hall = Game.Save.Get<GuildHall>();
			timer.OnTrigger.AddListener(OnTrigger);

			guild.QuestBoard.Add(new());
		}

		void OnTrigger() {
			var visitor = guild.Generate.Adventurer();
			hall.Freelancers.BinaryInsert(visitor);
			SpawnVisitor(visitor, visitor);
		}

		public void SpawnVisitor(Adventurer adventurer, IVisitor visitor) {
			var controller = Instantiate(visitorAiPrefab, spots.entrance.position, spots.entrance.rotation);
			Instantiate(visitor.Prefab, spots.entrance.position, spots.entrance.rotation, controller.transform);
			var ai = new FreelancerVisitorAI(adventurer, controller.Agent, spots);
			controller.SetVisitor(ai);
		}
	}
}

