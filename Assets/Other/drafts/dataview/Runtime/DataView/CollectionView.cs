using Drafts.Extensions;
using Drafts.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
namespace Drafts.UI {

	public class CollectionView : DataView<IEnumerable> {

		[SerializeField] protected DataHolder template;
		[SerializeField] protected UnityEvent onEmptySelection;
		[SerializeField] protected bool pooling;
		[SerializeField] protected FormattedText countText;

		[SerializeField] protected List<DataHolder> views = new();
		protected Stack<DataHolder> pool = new();
		public IReadOnlyList<DataHolder> Views => views;

		protected virtual void Awake() => template.TrySetActive(false);

		protected override void Subscribe() {
			Add(0, Data);
			if(Data is INotifyCollectionChanged c)
				c.CollectionChanged += CollectionChanged;
		}

		protected override void Unsubscribe() {
			RemoveAll();
			if(Data is INotifyCollectionChanged c)
				c.CollectionChanged -= CollectionChanged;
		}

		private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			switch(e.Action) {
				case NotifyCollectionChangedAction.Add: Add(e.NewStartingIndex, e.NewItems); break;
				case NotifyCollectionChangedAction.Move: throw new NotImplementedException("Move");
				case NotifyCollectionChangedAction.Remove: Remove(e.OldStartingIndex, e.OldItems); break;
				case NotifyCollectionChangedAction.Replace: Replace(e.NewStartingIndex, e.NewItems); break;
				case NotifyCollectionChangedAction.Reset: RemoveAll(); break;
				default: break;
			}
			countText.TrySetValue(Data.Count());
		}

		private void RemoveAll() {
			var selected = EventSystem.current?.currentSelectedGameObject;
			if(views.Any(v => v.gameObject == selected)) onEmptySelection.Invoke();
			views.Clear();
		}

		protected virtual DataHolder CreateItem(object data) {
			if(!pooling || pool.Count == 0) return template.Template(data);
			var view = pool.Pop();
			view.gameObject.SetActive(true);
			view.Data = data;
			return view;
		}

		protected virtual void RemoveItem(DataHolder view) {
			if(pooling) pool.Push(view);
			view.gameObject.SetActive(false);
		}

		private void Add(int index, IEnumerable items) {
			if(index < 0) index = views.Count;
			foreach(var item in items) {
				var v = CreateItem(item);

				if(v.TryGetComponent<IndexView>(out var id)) {
					if(Data is INotifyCollectionChanged c) {
						id.Dynamic = () => views.IndexOf(v);
						c.CollectionChanged += id.Refresh;
						id.onDestroy += () => c.CollectionChanged -= id.Refresh;
					} else {
						id.Data = index;
					}
				}

				v.name = index + ": " + template.name;
				views.Insert(index, v);
				v.transform.SetSiblingIndex(++index);
			}
		}

		private void Remove(int index, IEnumerable items) {
			var item = items.First();
			if(index < 0) index = views.FindIndex(d => d.Data == item);

			foreach(var _ in items) {
				SelectSiblingIfSelected(views[index]);
				RemoveItem(views[index]);
				views.RemoveAt(index);
			}
		}

		private void Replace(int index, IEnumerable itens) {
			foreach(var item in itens)
				views[index++].Data = item;
		}

		private void SelectSiblingIfSelected(DataHolder view) {
			if(!EventSystem.current) return;
			if(EventSystem.current.currentSelectedGameObject != view.gameObject) return;
			if(views.Count == 1) { onEmptySelection.Invoke(); return; }
			var index = views.IndexOf(view);
			index += index == views.Count - 1 ? -1 : +1;
			EventSystem.current.SetSelectedGameObject(views[index].gameObject);
		}

		public void ClearFilter() {
			foreach(var view in views) view.gameObject.SetActive(true);
		}

		public void Filter<T>(Predicate<T> predicate) {
			var index = GetSelectionIndex();
			foreach(var view in views)
				view.gameObject.SetActive(view.Data is T d && predicate(d));

			if(index < 0) return;
			var active = views.Where(v => v.gameObject.activeSelf);
			var element = active.ElementAtOrDefault(index) ?? active.Last();
			EventSystem.current.SetSelectedGameObject(element.gameObject);
		}

		int GetSelectionIndex() {
			var curr = EventSystem.current?.currentSelectedGameObject;
			var index = -1;
			var selected = views.FirstOrDefault(v => {
				if(v.gameObject.activeSelf) index++;
				return v.gameObject == curr;
			});
			return selected ? index : -1;
		}

		public void SetCount(int count) => Data = Enumerable.Range(0, count);
		public void SetCount(float count) => Data = Enumerable.Range(0, (int)count);
	}
}