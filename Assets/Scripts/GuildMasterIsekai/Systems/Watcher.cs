using System;
using UnityEngine;

namespace GuildMasterIsekai.Systems
{
    [Serializable]
    public class Watcher<T>
    {
        [SerializeField] private T value;
        public event Action<T> OnChanged;
        public Watcher() { }
        public Watcher(T value) => this.value = value;

        public T Value
        {
            get => value;
            set {
                this.value = value;
                OnChanged?.Invoke(value);
            }
        }

        public void SetSilently(T val) => value = val;
        public override string ToString() => $"({typeof(T).Name}) {value}";
    }
}