using Drafts;
using Drafts.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GuildMasterIsekai {
	public class CostumerView : DataView<Costumer> {
		[SerializeField] TextMeshProUGUI guid;
		[SerializeField] TextMeshProUGUI displayName;
		[SerializeField] Image icon;

		protected override void Repaint() {
			displayName.TrySetText(Data.Guid);
			displayName.TrySetText(Data.DisplayName);
			icon.TrySetSprite(Data.Icon);
		}
	}
}
