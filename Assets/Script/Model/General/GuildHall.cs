using Drafts;
using Drafts.Patterns;
using System;
using UnityEngine;

namespace GuildMasterIsekai {
	[Serializable]
	public class GuildHall : IJsonGameSave {
		[SerializeField] SimpleListWatcher<Quest> questBoard = new();
		[SerializeField] SimpleListWatcher<Costumer> costumers = new();
		[SerializeField] SimpleListWatcher<Adventurer> freelancers = new();
		[SerializeField] SimpleListWatcher<QuestRequest> requests = new();
		[SerializeField] SimpleListWatcher<ProblemRequest> problems = new();

		public SimpleListWatcher<Quest> QuestBoard => questBoard;
		public SimpleListWatcher<Costumer> Costumers => costumers;
		public SimpleListWatcher<Adventurer> Freelancers => freelancers;
		public SimpleListWatcher<QuestRequest> QuestRequests => requests;
		public SimpleListWatcher<ProblemRequest> ProblemRequests => problems;
	}
}