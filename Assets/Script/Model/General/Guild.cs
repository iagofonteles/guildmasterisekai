using Drafts;
using Drafts.Patterns;
using System;
using UnityEngine;

namespace GuildMasterIsekai {
	[Serializable]
	public class Guild : IJsonGameSave {
		[SerializeField] string rank = "F";
		[SerializeField] Watcher<int> gold;

		Rank _rank;
		public Rank Rank => _rank ??= Game.Database.Find<Rank>(rank);
		public Watcher<int> Gold => gold;

		public SimpleListWatcher<Adventurer> Adventurers = new();

		Generator generator;
		public Generator Generator => generator ??= new Generator(this);
	}
}