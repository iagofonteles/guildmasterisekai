using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;
using Drafts.Linq;
using UnityEngine.Events;
using System.Collections.Specialized;

namespace Drafts.UI {
	public class PaginatedCollectionView : DataView<IEnumerable> {
		public enum NullHandler { Deactivate, Invisible, Nothing }

		[SerializeField] protected DataHolder template;
		[Tooltip("{0} current, {1} max")]
		[SerializeField] protected FormattedText pageCount;
		public int itensPerPage = 10;
		[SerializeField] NullHandler nullHandler;
		[SerializeField] protected UnityEvent onEmptySelection;
		[SerializeField, ReadOnly] protected List<DataHolder> views = new();

		public int Count { get; private set; }
		public int CurrentPage { get; private set; }
		public IReadOnlyList<DataHolder> Views => views;
		public int MaxPage { get; private set; }

		void Awake() {
			template.gameObject.SetActive(false);
			for(int i = 0; i < itensPerPage; i++)
				views.Add(Instantiate(template, template.transform.parent));
		}

		protected override void Subscribe() {
			CurrentPage = 0;
			Count = Data.Count();
			Recalculate();
			if(Data is INotifyCollectionChanged c)
				c.CollectionChanged += CollectionChanged;
		}

		protected override void Unsubscribe() {
			CurrentPage = 0;
			Count = 0;
			Recalculate();
			if(Data is INotifyCollectionChanged c)
				c.CollectionChanged -= CollectionChanged;
		}

		protected virtual void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			Count = e.Action switch {
				NotifyCollectionChangedAction.Add => Count + e.NewItems.Count,
				NotifyCollectionChangedAction.Remove => Count -= e.NewItems.Count,
				NotifyCollectionChangedAction.Reset => 0,
			};
			Recalculate();
			Refresh();
		}

		private void Recalculate() {
			MaxPage = Mathf.CeilToInt((float)Count / itensPerPage) - 1;
			if(CurrentPage > MaxPage) CurrentPage = MaxPage;
		}

		protected override void Repaint() {
			var itens = Data.Skip(itensPerPage * CurrentPage).GetEnumerator();
			var currentIndex = itensPerPage * CurrentPage;
			foreach(var view in views) {
				var item = itens.MoveNext() ? itens.Current : null;
				if(view.Data == item) continue;
				view.gameObject.SetActive(item != null);
				view.Data = item;
				if(view.TryGetComponent<IndexView>(out var id)) id.Data = currentIndex++;
			}
			pageCount.TrySetValue(CurrentPage + 1, MaxPage + 1);
		}

		public void NextPage() { if(CurrentPage < MaxPage) CurrentPage++; Refresh(); }
		public void PreviousPage() { if(CurrentPage > 0) CurrentPage--; Refresh(); }
		public void GoToPage(int page) { if(page <= MaxPage) CurrentPage = page; Refresh(); }
	}
}
