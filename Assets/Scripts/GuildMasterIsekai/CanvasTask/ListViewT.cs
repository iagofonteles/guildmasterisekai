using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using TileGrid.UI;
using UnityEngine.UIElements;

namespace Drafts.UIElements
{
    public sealed class ListView<T> : ViewElement<IEnumerable> where T : VisualElement, IViewElement, new()
    {
        private readonly UnityEngine.UIElements.ListView _listView;
        private readonly HashSet<T> _views = new();
        private INotifyCollectionChanged _notifications;
        private Predicate<object> _filter;

        public event Action<int> CountChanged;
        public Action<T> ConfigureItem { get; set; }
        public IEnumerable<T> Views => _views.Where(view => view.Data != null);

        public Predicate<object> Filter
        {
            get => _filter;
            set { _filter = value; Refresh(); }
        }

        public ListView()
        {
            AddToClassList("list");
            _listView = new UnityEngine.UIElements.ListView
            {
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                selectionType = SelectionType.None,
                makeItem = MakeItem,
                bindItem = (element, index) => ((IViewElement)element).Data = _listView.itemsSource[index],
                unbindItem = (element, index) => ((IViewElement)element).Data = null,
                destroyItem = element => { ((IViewElement)element).Data = null; _views.Remove((T)element); }
            };
            _listView.AddToClassList("list");
            Add(_listView);
            RegisterCallback<AttachToPanelEvent>(_ => { if (Data != null) { Subscribe(); Refresh(); } });
            RegisterCallback<DetachFromPanelEvent>(_ => { Unsubscribe(); ReleaseRows(); });
        }

        private VisualElement MakeItem()
        {
            var view = new T();
            ConfigureItem?.Invoke(view);
            _views.Add(view);
            return view;
        }

        protected override void Subscribe()
        {
            Unsubscribe();
            _notifications = Data as INotifyCollectionChanged;
            if (_notifications != null) _notifications.CollectionChanged += Changed;
        }

        protected override void Unsubscribe()
        {
            if (_notifications != null) _notifications.CollectionChanged -= Changed;
            _notifications = null;
        }

        private void Changed(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (_filter == null && args.Action == NotifyCollectionChangedAction.Replace &&
                args.NewStartingIndex >= 0 && args.NewItems.Count == 1)
            {
                _listView.itemsSource[args.NewStartingIndex] = args.NewItems[0];
                _listView.RefreshItem(args.NewStartingIndex);
                CountChanged?.Invoke(_listView.itemsSource.Count);
                return;
            }
            Refresh();
        }

        public override void Refresh() => _listView.RefreshItems();


        public override void SetSelected(object selected)
        {
            foreach (var view in Views) view.SetSelected(selected);
        }
    }
}
