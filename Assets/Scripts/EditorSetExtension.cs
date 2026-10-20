using System.Reflection;
using UnityEngine;
using static System.Reflection.BindingFlags;

namespace GuildMasterIsekai
{
    public static class EditorSetExtension
    {
        private const BindingFlags Flags = NonPublic | Instance | Public;

        public static void EditorSet<T>(this T obj, string field, object value) where T : Object
        {
            var info = typeof(T).GetField(field, Flags);
            if (info!.GetValue(obj) == value) return;
            info!.SetValue(obj, value);
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(obj);
#endif
        }
    }
}