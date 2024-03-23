using System;
using UnityEngine;
using System.Collections.Specialized;
namespace Drafts {
	public class IndexView : DataView<int> {
		[SerializeField] FormattedText index;
		public Func<int> Dynamic;
		public override int Data { get => Dynamic?.Invoke() ?? base.Data; set => base.Data = value; }
		protected override void Repaint() => index.TrySetValue(Data);
		public void Refresh(object sender, NotifyCollectionChangedEventArgs args) => Refresh();
	}
}
