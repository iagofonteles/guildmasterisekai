using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using TileGrid.UI;
using UnityEngine.UIElements;

namespace Drafts.UIElements
{
    public sealed class ListView<T> : ViewElement<IEnumerable> where T : IViewElement, new()
    {
        private readonly ListView _listView;
        private readonly Wrapper _wrapper = new();
        public Label EmptyLabel { get; } = new("Empty");

        public ListView()
        {
            Add(_listView = new(null, -1, MakeItem, BindItem));
            Add(EmptyLabel);
        }

        public IEnumerable<T> Views => _listView.contentContainer.Children().OfType<T>();

        protected override void Subscribe()
        {
            _listView.itemsSource = Data switch
            {
                IList a => a,
                IReadOnlyList<object> b => _wrapper.SetData(b),
                _ => throw new InvalidDataException(Data.GetType().Name)
            };

            if (Data is INotifyCollectionChanged n)
                n.CollectionChanged += Changed;
        }

        protected override void Unsubscribe()
        {
            if (Data is INotifyCollectionChanged n)
                n.CollectionChanged -= Changed;
        }

        private static VisualElement MakeItem() => new T().Element;
        private void BindItem(VisualElement element, int index) 
            => ((IViewElement)element).Data = _listView.itemsSource[index];

        //TODO refresh only revlevant
        private void Changed(object sender, NotifyCollectionChangedEventArgs args) => Refresh();

        public override void Refresh()
        {
            _listView.RefreshItems();
            EmptyLabel.visible = _listView.itemsSource.Count == 0;
        }

        public override void SetSelected(object selected)
        {
            foreach (var e in _listView.contentContainer.Children())
                ((IViewElement)e).SetSelected(selected);
        }
    }

    internal class Wrapper : IList
    {
        private IReadOnlyList<object> _data;

        public Wrapper SetData(IReadOnlyList<object> d)
        {
            _data = d;
            return this;
        }

        public int Count => _data.Count;
        public bool IsSynchronized => throw new InvalidOperationException();
        public object SyncRoot => throw new InvalidOperationException();
        public bool IsFixedSize => true;
        public bool IsReadOnly => true;
        public IEnumerator GetEnumerator() => _data.GetEnumerator();
        public void CopyTo(Array array, int index) => throw new InvalidOperationException();
        public int Add(object value) => throw new InvalidOperationException();
        public void Clear() => throw new InvalidOperationException();
        public bool Contains(object value) => throw new InvalidOperationException();
        public int IndexOf(object value) => throw new InvalidOperationException();
        public void Insert(int index, object value) => throw new InvalidOperationException();
        public void Remove(object value) => throw new InvalidOperationException();
        public void RemoveAt(int index) => throw new InvalidOperationException();

        public object this[int index] { get => _data[index]; set => throw new InvalidOperationException(); }
    }
}