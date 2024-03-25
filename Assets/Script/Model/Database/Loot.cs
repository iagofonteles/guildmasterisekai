using UnityEngine;
namespace GuildMasterIsekai {
	[CreateAssetMenu(menuName = "Guild Master/Loot")]
	public class Loot : DatabaseEntrySO {
		[SerializeField] eStat mainStat;
		public eStat MainStat => mainStat;
	}
}
