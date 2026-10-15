using GuildMasterIsekai.System;
using UnityEngine;

namespace GuildMasterIsekai.Database
{
    [CreateAssetMenu(menuName = "Guild Master/Stat")]
    public class StatSo : DatabaseEntrySo { }

    public enum Stat { Power, Knowledge, Resistance }
}