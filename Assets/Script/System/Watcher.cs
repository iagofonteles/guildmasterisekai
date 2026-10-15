using System;
using UnityEngine;

namespace GuildMasterIsekai.System
{
    public interface IWatcher
    {
        object Value { get; }
        event Action<object> OnChanged;
    }

    public interface IWatcher<out T> : IWatcher
    {
        new T Value { get; }
    }

    [Serializable]
    public class Watcher<T> : IWatcher<T>
    {
        [SerializeField] private T value;

        public Watcher() { }

        public Watcher(T value)
        {
            this.value = value;
        }

        public event Action<object> OnChanged;

        public T Value
        {
            get => value;
            set {
                this.value = value;
                OnChanged?.Invoke(value);
            }
        }

        object IWatcher.Value => value;
        public void SetSilently(T val) => value = val;
        public override string ToString() => $"({typeof(T).Name}) {value}";
    }
}