using Drafts;
using Drafts.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GuildMasterIsekai {

	public class QuestView : DataView<Quest> {
		[SerializeField] TextMeshProUGUI guid;
		[SerializeField] TextMeshProUGUI displayName;
		[SerializeField] TextMeshProUGUI description;
		//[SerializeField] string rank;
		[SerializeField] Image icon;
		[SerializeField] StatsView requisite;
		[SerializeField] FormattedText fee;
		//[SerializeField] Reward reward;

		protected override void Repaint() {
			guid.TrySetText(Data.Guid);
			displayName.TrySetText(Data.DisplayName);
			description.TrySetText(Data.Description);
			//rank = rank.name;
			icon.TrySetSprite(Data.Icon);
			requisite.TrySetData(Data.Requisite);
			fee.TrySetValue(Data.Fee);
			//reward = reward;
		}
	}
}
