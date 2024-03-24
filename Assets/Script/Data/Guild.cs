using Drafts;
using System;
using System.Collections.Generic;

namespace GuildMasterIsekai {
	[Serializable]
	public class Guild : IJsonGameSave {
		public List<Adventurer> Adventurers = new();
		public List<Quest> QuestBoard = new();

		Generator generator;
		public Generator Generate => generator ??= new Generator(this);
	}

	[Serializable]
	public class GuildHall : IJsonGameSave {
		public List<Costumer> Costumers = new();
		public List<Adventurer> Freelancers = new();
	}
}