using Drafts;
using Drafts.Databases;
namespace GuildMasterIsekai {

	public static class Game {

		public static Database<DatabaseEntrySO> Database { get; private set; }
		public static GameSave Save { get; private set; }

		static Game() => Reset();

		public static void Reset() {
			Database = new(new ResourcesEntryProvider<DatabaseEntrySO>("{0}"));
			Save = new("../Save");
			Save.Load("Test");
		}
	}
}