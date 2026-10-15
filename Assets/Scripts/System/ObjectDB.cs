using System.Collections.Generic;

namespace GuildMasterIsekai.System
{
    public static class ObjectDB<T>
    {
        private static readonly Dictionary<uint, T> _items = new();
        public static T Get(uint id) => _items.GetValueOrDefault(id);
        public static void Add(uint id, T item) => _items.Add(id, item);
        public static bool Remove(uint id) => _items.Remove(id);
    }
}