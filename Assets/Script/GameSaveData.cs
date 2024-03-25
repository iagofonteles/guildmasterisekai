using Drafts;
using UnityEngine;

namespace GuildMasterIsekai {
	public class GameSaveData : MonoBehaviour {
		enum Data {
			Guild,
			GuildHall,
			GuildRank,
			Adventurers,
			Freelancers,
			Costumers,
			QuestBoard,
			QuestRequests,
			ProblemRequests,
		}

		[SerializeField] Data data;
		void Start() {
			var guild = Game.Save.Get<Guild>();
			var hall = Game.Save.Get<GuildHall>();
			
			GetComponent<IData>().Data = data switch {
				Data.Guild => guild,
				Data.GuildHall => hall,
				Data.GuildRank => guild.Rank,
				Data.Adventurers => guild.Adventurers,
				Data.Freelancers => hall.Freelancers,
				Data.Costumers => hall.Costumers,
				Data.QuestBoard => hall.QuestBoard,
				Data.QuestRequests => hall.QuestRequests,
				Data.ProblemRequests => hall.ProblemRequests,
				_ => throw new System.NotImplementedException(),
			};
		}
	}
}