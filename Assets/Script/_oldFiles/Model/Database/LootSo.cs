using UnityEngine;
namespace GuildMasterIsekai {
	[CreateAssetMenu(menuName = "Guild Master/Loot")]
	public class LootSo : DatabaseEntrySo {
		[SerializeField] eStat mainStat;
		public eStat MainStat => mainStat;
	}
}
