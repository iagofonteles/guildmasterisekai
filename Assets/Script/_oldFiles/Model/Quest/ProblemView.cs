using Drafts.DataView;
using TMPro;
using UnityEngine;

namespace GuildMasterIsekai {
	public class ProblemView : DataView<Problem> {
		[SerializeField] private TMP_Text id;
		[SerializeField] private CollectionView outcomes;

		protected override void Subscribe()
		{
			base.Subscribe();
			id.TrySetText(Data.Id);
			outcomes.TrySetData(Data.Outcomes);
		}
	}
}