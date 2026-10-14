using Drafts;
using Drafts.Extensions;
using TMPro;
using UnityEngine;

namespace GuildMasterIsekai {

	public class QuestRequestView : DataView<QuestRequest> {
		[SerializeField] TextMeshProUGUI text;

		protected override void Repaint() {
			text.TrySetText(Data.Quest.Guid);
		}

		public void Accept() => Data.Reply(Data.Quest);
		public void Refuse() => Data.Reply(null);
	}
}