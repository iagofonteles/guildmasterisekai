using System.Collections.Generic;

namespace GuildMasterIsekai
{
    public static class RandomUtility
    {
        public static T Random<T>(this IReadOnlyList<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        public static List<T> Random<T>(this IReadOnlyList<T> list, int amount)
        {
            var result = new List<T>();
            for (var i = 0; i < amount; i++)
                result.Add(list.Random());
            return result;
        }

        public static T Random<T>(this IReadOnlyCollection<T> col)
        {
            var n = UnityEngine.Random.Range(0, col.Count);
            foreach (var item in col)
                if (n-- == 0)
                    return item;
            return default;
        }
    }
}