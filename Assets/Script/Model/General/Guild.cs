using Drafts;
using Drafts.Patterns;
using System;
using UnityEngine;

namespace GuildMasterIsekai {
	[Serializable]
	public class Guild : IJsonGameSave {
		[SerializeField] string rank = "F";

		Rank _rank;
		public Rank Rank => _rank ??= Game.Database.Find<Rank>(rank);

		public SimpleListWatcher<Adventurer> Adventurers = new();

		Generator generator;
		public Generator Generator => generator ??= new Generator(this);
	}
}