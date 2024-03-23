#if DRAFTS_DATAVIEW
using UnityEngine;
using TMPro;
using Drafts.Extensions;
using UnityEngine.Events;

namespace Drafts.Inventory {
	public class ISlotView : DataView<ISlot> {

		[SerializeField] DataHolder itemView;
		[SerializeField] TextMeshProUGUI countText;
		[SerializeField] GameObject favoriteIcon;
		[SerializeField] UnityEvent<bool> OnAmountZero;

		protected override void Subscribe() {
			itemView.TrySetData(Data.Item);
			Data.OnChanged += Refresh;
		}

		protected override void Unsubscribe() {
			itemView.TrySetData(default);
			Data.OnChanged -= Refresh;
		}

		void Refresh(int delta) => Refresh();
		protected override void Repaint() {
			countText.TrySetText(Data.Amount);
			favoriteIcon.TrySetActive(Data.Favorite);
			OnAmountZero?.Invoke(Data.Amount == 0);
		}
	}
}
#endif