using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine.UIElements;

namespace GuildMasterIsekai.CanvasTask
{
    public sealed class UiCollectionBinding<T> : IDisposable
    {
        private readonly VisualElement _root;
        private readonly Func<T, UiBinding<T>> _createRow;
        private readonly List<UiBinding<T>> _rows = new();
        private IEnumerable<T> _source;
        private INotifyCollectionChanged _notifications;
        private bool _disposed;

        public UiCollectionBinding(VisualElement root, Func<T, UiBinding<T>> createRow)
        {
            _root = root ?? throw new ArgumentNullException(nameof(root));
            _createRow = createRow ?? throw new ArgumentNullException(nameof(createRow));
        }

        public event Action<int> CountChanged;
        public Predicate<T> Filter { get; set; }

        public void Bind(IEnumerable<T> data)
        {
            if (_disposed) throw new ObjectDisposedException(GetType().Name);
            if (_notifications != null) _notifications.CollectionChanged -= Changed;
            _source = data;
            _notifications = data as INotifyCollectionChanged;
            if (_notifications != null) _notifications.CollectionChanged += Changed;
            Refresh();
        }

        public void Refresh()
        {
            if (_disposed) return;
            ClearRows();
            var snapshot = _source?.ToArray() ?? Array.Empty<T>();
            foreach (var item in snapshot)
            {
                if (Filter != null && !Filter(item)) continue;
                var row = _createRow(item);
                _rows.Add(row);
                _root.Add(row.Root);
                row.Bind(item);
            }
            if (_rows.Count == 0) _root.Add(new Label("No entries.") { name = "emptyState" });
            CountChanged?.Invoke(snapshot.Length);
        }

        private void Changed(object sender, NotifyCollectionChangedEventArgs args) => Refresh();

        private void ClearRows()
        {
            foreach (var row in _rows) row.Dispose();
            _rows.Clear();
            _root.Clear();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            if (_notifications != null) _notifications.CollectionChanged -= Changed;
            _notifications = null;
            _source = null;
            ClearRows();
            CountChanged = null;
        }
    }
}