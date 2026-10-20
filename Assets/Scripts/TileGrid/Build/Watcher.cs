using System;
using UnityEngine;

namespace TileGrid.Build
{
    [Serializable]
    public class Watcher<T>
    {
        [SerializeField] private T value;
        public Watcher(T value = default) => this.value = value;
        public event Action<T> OnChanged;

        public T Value
        {
            get => value;
            set {
                this.value = value;
                OnChanged?.Invoke(value);
            }
        }
    }
}