using GuildMasterIsekai.System;
using UnityEngine;

namespace GuildMasterIsekai.Database
{
    [CreateAssetMenu(menuName = "Guild Master/Loot")]
    public class LootSo : DatabaseEntrySo
    {
        [SerializeField] private Stat mainStat;
        public Stat MainStat => mainStat;
    }
}