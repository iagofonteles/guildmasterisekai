//using Drafts.Inventory;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using UnityEngine;

//namespace Drafts.UI {

//	public class CollectionView : DataView<SlotInventory> {

//		[SerializeField] Transform template;

//		IDataView _template;

//		List<IDataView> views = new List<IDataView>();

//		private void Awake() => _template = template.GetComponent<IDataView>();

//		protected override void Subscribe() {
//			if(Data is IList l) Add(0, l);
//			Data.CollectionChanged += CollectionChanged;
//		}

//		protected override void Unsubscribe() {
//			if(Data is IList l) Remove(0, l);
//			Data.CollectionChanged -= CollectionChanged;
//		}

//		private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
//			switch(e.Action) {
//				case NotifyCollectionChangedAction.Add: Add(e.NewStartingIndex, e.NewItems); break;
//				case NotifyCollectionChangedAction.Move: new NotImplementedException("Move"); break;
//				case NotifyCollectionChangedAction.Remove: Remove(e.OldStartingIndex, e.OldItems); break;
//				case NotifyCollectionChangedAction.Replace: new NotImplementedException("Replace"); break;
//				case NotifyCollectionChangedAction.Reset: RemoveAll(); break;
//				default: break;
//			}
//		}

//		private void RemoveAll() { foreach(var view in views) Destroy(view.gameObject); views.Clear(); }

//		private void Remove(int index, IList items) {
//			for(int i = 0; i < items.Count; i++) {
//				Destroy(views[index].gameObject);
//				views.RemoveAt(index);
//			}
//		}

//		private void Add(int index, IList items) {
//			for(int i = items.Count - 1; i >= 0; i--) {
//				var v = Instantiate(template, template.parent).GetComponent<IDataView>();
//				v.Data = items[i];
//				views.Insert(index, v);
//			}
//		}
//	}
//}
