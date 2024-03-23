//#if DRAFTS_DATAVIEW
//using Drafts.Patterns;
//using UnityEngine.Events;
//namespace Drafts.UI {
//	public class IntStatWatcherView : StatWatcherView<int> {
//		public UnityEvent<float> normalized;
//		protected override void Repaint() {
//			var n = Data as INumStatWatcher<int>;
//			var c = Data.Value == Data.Base ? uCol : Data.Value < Data.Base ? lCol : mCol;
//			format.text = string.Format(defValue1, c, Data.Value, Data.Base, n?.Min, n?.Max);
//			normalized.Invoke(Data.Value / (float)Data.Base);
//		}
//	}
//}
//#endif