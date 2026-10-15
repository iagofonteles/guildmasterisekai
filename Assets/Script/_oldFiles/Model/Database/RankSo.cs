using System;
using System.Collections.Generic;
using UnityEngine;

namespace GuildMasterIsekai
{
    public enum Rank { F,E,D,C,B,A,S }
    
    [CreateAssetMenu(menuName = "Game/Rank")]
    public class RankSo : DatabaseEntrySo, IComparable<RankSo>
    {
        private static IReadOnlyList<RankSo> _ranks;

        [SerializeField] private int order;
        public int Order => order;

        public int CompareTo(RankSo other) => order.CompareTo(other.order);

        public static bool operator >(RankSo a, RankSo b) => a.order > b.order;
        public static bool operator <(RankSo a, RankSo b) => a.order < b.order;
        public static bool operator >=(RankSo a, RankSo b) => a.order >= b.order;
        public static bool operator <=(RankSo a, RankSo b) => a.order <= b.order;

        public static RankSo Get(Rank r) => (_ranks ??= Game.Database.GetAll<RankSo>())[(int)r];
    }
}