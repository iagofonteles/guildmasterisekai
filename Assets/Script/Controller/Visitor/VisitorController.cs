using Drafts;
using UnityEngine;

namespace GuildMasterIsekai {

	public class VisitorController : MonoBehaviour {
		[SerializeField] TimerComponent adventurerTimer;
		[SerializeField] TimerComponent costumerTimer;
		[SerializeField] HallSpots spots;
		[SerializeField, Prefab] VisitorAI visitorAiPrefab;

		Guild guild;
		GuildHall hall;

		void Start() {
			guild = Game.Save.Get<Guild>();
			hall = Game.Save.Get<GuildHall>();
			adventurerTimer.OnTrigger.AddListener(OnTriggerAdventurer);
			costumerTimer.OnTrigger.AddListener(OnTriggerCostumer);
		}

		void OnTriggerAdventurer() => SpawnVisitor(guild.Generator.NewAdventurer());
		void OnTriggerCostumer() => SpawnVisitor(guild.Generator.NewCostumer());
		public void SpawnVisitor(IVisitor visitor) {
			var controller = Instantiate(visitorAiPrefab, spots.Entrance, visitorAiPrefab.transform.rotation, transform);
			Instantiate(visitor.Prefab, controller.transform);
			controller.SetVisitor(visitor.GetAI(controller.Agent, spots));
		}
	}
}

