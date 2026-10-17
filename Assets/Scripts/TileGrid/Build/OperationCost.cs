using System;
using System.Collections.Generic;

namespace GuildMasterIsekai.TileGrid
{
    /// <summary>Positive amounts are costs consumed by Pay and restored by Refund; negative amounts reverse that direction.</summary>
    public sealed class OperationCost
    {
        private readonly Dictionary<object, int> items = new();
        public IReadOnlyDictionary<object, int> Items => items;

        public void AddItem(object item, int amount)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            items.TryGetValue(item, out var previous);
            items[item] = checked(previous + amount);
        }
    }
}