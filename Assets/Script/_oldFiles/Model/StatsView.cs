using Drafts.DataView;
using TMPro;
using UnityEngine;

namespace GuildMasterIsekai {
	public class StatsView : DataView<Stats> {
		[SerializeField] private TMP_Text power;
		[SerializeField] private TMP_Text knowledge;
		[SerializeField] private TMP_Text vigor;

		protected override void Subscribe()
		{
			base.Subscribe();
			power.TrySetText(Data.Power);
			vigor.TrySetText(Data.Vigor);
			knowledge.TrySetText(Data.Knowledge);
		}
	}
}
