#if DRAFTS_DATAVIEW
using Drafts.Patterns;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Drafts.UI {
	public class IStatView : DataView<ISimpleStat> {

		[Header("{0} color	{1} value	{2} base	{3} min	{4} max")]
		[SerializeField] protected TextMeshProUGUI format;
		[SerializeField] Color unchanged = Color.white;
		[SerializeField] Color less = Color.white;
		[SerializeField] Color more = Color.white;
		[SerializeField] UnityEvent<float> OnValueChanged;
		[SerializeField] UnityEvent<float> percent;

		protected string defValue1 = "";
		protected string uCol, lCol, mCol;

		private void Awake() {
			if(format) defValue1 = format.text;
			uCol = $"<color=#{ColorUtility.ToHtmlStringRGBA(unchanged)}>";
			lCol = $"<color=#{ColorUtility.ToHtmlStringRGBA(less)}>";
			mCol = $"<color=#{ColorUtility.ToHtmlStringRGBA(more)}>";
		}

		protected override void Subscribe() => Data.OnModified += Refresh;
		protected override void Unsubscribe() => Data.OnModified -= Refresh;

		protected override void Repaint() {
			if(!format) return;

			var col = Data.Percent < 1f ? lCol : Data.Percent > 1 ? mCol : uCol;

			format.text = Data is IStat st
				? string.Format(defValue1, col, st.Total, Data.Base, st.Min, st.Max)
				: string.Format(defValue1, col, Data.Current, Data.Base, -1, -1);

			percent?.Invoke(Data.Percent);
			OnValueChanged?.Invoke(Data.Current is int i ? i : (float)Data.Current);
		}
	}
}
#endif
