using System;
using System.Collections.Generic;
using GuildMasterIsekai.Systems;
using UnityEngine.UIElements;

namespace GuildMasterIsekai.CanvasTask
{
    public sealed class UiBinding<T> : IDisposable
    {
        private readonly Action<T, UiBinding<T>> _render;
        private readonly List<Action> _cleanup = new();
        private bool _disposed;

        public UiBinding(VisualElement root, Action<T, UiBinding<T>> render)
        {
            Root = root ?? throw new ArgumentNullException(nameof(root));
            _render = render ?? throw new ArgumentNullException(nameof(render));
        }

        public VisualElement Root { get; }
        public T Data { get; private set; }

        public void Bind(T data)
        {
            if (_disposed) throw new ObjectDisposedException(GetType().Name);
            ClearSubscriptions();
            Data = data;
            _render(data, this);
        }

        public void Own(IDisposable binding) => OnUnbind(binding.Dispose);
        public void OnUnbind(Action action) => _cleanup.Add(action);

        public void Watch(IDisplay display, Action refresh)
        {
            if (display == null) return;
            display.OnChanged += refresh;
            OnUnbind(() => display.OnChanged -= refresh);
        }

        public void Watch(IWatcher watcher, Action refresh)
        {
            if (watcher == null) return;
            void Changed(object value) => refresh();
            watcher.OnChanged += Changed;
            OnUnbind(() => watcher.OnChanged -= Changed);
        }

        private void ClearSubscriptions()
        {
            for (var i = _cleanup.Count - 1; i >= 0; i--) _cleanup[i]();
            _cleanup.Clear();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            ClearSubscriptions();
            Data = default;
        }
    }
}
