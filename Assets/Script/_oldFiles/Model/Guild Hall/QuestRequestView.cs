using Drafts.DataView;
using TMPro;
using UnityEngine;

namespace GuildMasterIsekai {

	public class QuestRequestView : DataView<QuestRequest> {
		[SerializeField] private TMP_Text text;

		protected override void Subscribe()
		{
			base.Subscribe();
			text.TrySetText(Data.Quest.Id);
		}

		public void Accept() => Data.Reply(Data.Quest);
		public void Refuse() => Data.Reply(null);
	}
}