using Drafts;
using UnityEngine;

namespace GuildMasterIsekai {
	public class StatsView : DataView<Stats> {
		[SerializeField] FormattedText power;
		[SerializeField] FormattedText knoledge;
		[SerializeField] FormattedText vigor;

		protected override void Repaint() {
			power.TrySetValue(Data.Power);
			vigor.TrySetValue(Data.Vigor);
			knoledge.TrySetValue(Data.Knoledge);
		}
	}
}
