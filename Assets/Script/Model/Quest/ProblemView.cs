using Drafts;
using Drafts.Extensions;
using Drafts.UI;
using TMPro;
using UnityEngine;

namespace GuildMasterIsekai {
	public class ProblemView : DataView<Problem> {
		[SerializeField] TextMeshProUGUI guid;
		[SerializeField] CollectionView outcomes;

		protected override void Repaint() {
			guid.TrySetText(Data.Guid);
			outcomes.TrySetData(Data.Outcomes);
		}
	}
}