using UnityEngine;
namespace GuildMasterIsekai {
	[CreateAssetMenu(menuName = "Guild Master/Loot")]
	public class LootSo : DatabaseEntrySo {
		[SerializeField] private Stat mainStat;
		public Stat MainStat => mainStat;
	}
}
