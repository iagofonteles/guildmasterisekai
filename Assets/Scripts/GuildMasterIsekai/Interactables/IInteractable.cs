using System;
using System.Collections.Generic;
using System.Linq;
using TileGrid.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace GuildMasterIsekai.Interactables
{
    public interface IInteractable
    {
        private static readonly Dictionary<Type, object> _filtered = new();
        public static void ClearCache() => _filtered.Clear();

        public static IReadOnlyCollection<T> GetActive<T>()
        {
            var t = typeof(T);
            if (!_filtered.TryGetValue(t, out var set))
                _filtered[t] = set = new HashSet<T>(InteractableComponent.Instances.OfType<T>());
            return (HashSet<T>)set;
        }

        void __setTransform(Transform t);
        Transform Transform { get; }
        void SubscribeUI(PanelRenderer ui);
    }
}