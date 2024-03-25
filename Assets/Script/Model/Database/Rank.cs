using Drafts;
using System;
using UnityEngine;

namespace GuildMasterIsekai {

	public class RankAttribute : SearchAttribute {
		public RankAttribute() : base(new AssetNameSearchSettings(typeof(Rank)), true) { }
	}

	[CreateAssetMenu(menuName = "Guild Master/Rank")]
	public class Rank : DatabaseEntrySO, IComparable<Rank> {
		[SerializeField] int order;
		public int Order => order;

		public int CompareTo(Rank other) => order.CompareTo(other.order);

		public static bool operator >(Rank a, Rank b) => a.order > b.order;
		public static bool operator <(Rank a, Rank b) => a.order < b.order;
		public static bool operator >=(Rank a, Rank b) => a.order >= b.order;
		public static bool operator <=(Rank a, Rank b) => a.order <= b.order;
	}
}