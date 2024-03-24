using Drafts;
using UnityEngine;

namespace GuildMasterIsekai {

	public class VisitorController : MonoBehaviour {
		[SerializeField] TimerComponent timer;
		[SerializeField] Transform entrance;
		[SerializeField] Transform balcon;
		[SerializeField, Prefab] VisitorAI visitorAiPrefab;

		Guild guild;
		GuildHall guildHall;

		void Start() {
			guild = Game.Save.Get<Guild>();
			guildHall = Game.Save.Get<GuildHall>();
			timer.OnTrigger.AddListener(OnTrigger);
		}

		// Update is called once per frame
		void OnTrigger() {
			var visitor = guild.Generate.Adventurer();
			guildHall.Freelancers.BinaryInsert(visitor);
			SpawnVisitor(visitor);
		}

		public void SpawnVisitor(IVisitor visitor) {
			var ai = Instantiate(visitorAiPrefab, entrance.position, entrance.rotation);
			var model = Instantiate(visitor.Prefab, entrance.position, entrance.rotation, ai.transform);
			ai.SetVisitor(visitor, balcon);
		}
	}
}

