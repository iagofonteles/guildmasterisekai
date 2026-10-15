using Drafts;
using UnityEngine;

namespace GuildMasterIsekai {

	public class TimerComponent : MonoBehaviour
	{
		
	}
	public class VisitorController : MonoBehaviour {
		[SerializeField] private TimerComponent adventurerTimer;
		[SerializeField] private TimerComponent costumerTimer;
		[SerializeField] private HallSpots spots;
		[SerializeField, Prefab] private VisitorAI visitorAiPrefab;

		private Guild guild;
		private GuildHall hall;

		private void Start() {
			guild = Game.Save.Get<Guild>();
			hall = Game.Save.Get<GuildHall>();
			adventurerTimer.OnTrigger.AddListener(OnTriggerAdventurer);
			costumerTimer.OnTrigger.AddListener(OnTriggerCostumer);
		}

		private void OnTriggerAdventurer() => SpawnVisitor(guild.Generator.NewAdventurer());
		private void OnTriggerCostumer() => SpawnVisitor(guild.Generator.NewCostumer());
		public void SpawnVisitor(IVisitor visitor) {
			var controller = Instantiate(visitorAiPrefab, spots.Entrance, visitorAiPrefab.transform.rotation, transform);
			Instantiate(visitor.Prefab, controller.transform);
			controller.SetVisitor(visitor.GetAI(controller.Agent, spots));
		}
	}
}

