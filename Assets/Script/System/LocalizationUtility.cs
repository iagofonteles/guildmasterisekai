using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Localization;
using Object = UnityEngine.Object;

namespace GuildMasterIsekai.System
{
    public static class LocalizationUtility
    {
        public static string GetString(this LocalizedString s, Object ctx, [CallerMemberName] string m = null)
        {
            try
            {
                return s.GetLocalizedString();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"String not found: {s.TableReference.TableCollectionName}.{s.TableEntryReference.Key}", ctx);
                Debug.LogWarning(e.Message, ctx);
                return $"[{ctx.name}.{m}]";
            }
        }
    }
}