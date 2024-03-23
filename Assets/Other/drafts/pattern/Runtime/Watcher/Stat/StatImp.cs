using System;
using UnityEngine;

namespace Drafts.Patterns {

	[Serializable]
	public class StatInt : Stat<int> {
		public StatInt(int @base = 0, int min = int.MinValue, int max = int.MaxValue) : this(null, @base, min, max) { }
		public StatInt(object owner, int @base = 0, int min = int.MinValue, int max = int.MaxValue) : base(owner) {
			Base.Value = Current.Value = @base; Min.Value = min; Max.Value = max; Mult.Value = 1;
		}

		protected override int GetTotal() {
			var t = (Current.Value + Add.Value) * Mult.Value;
			return Mathf.Clamp(t, Min.Value, Max.Value);
		}
	}

	[Serializable]
	public class StatFloat : Stat<float> {
		public StatFloat(float @base = 0, float min = float.MinValue, float max = float.MaxValue) : this(null, @base, min, max) { }
		public StatFloat(object owner, float @base = 0, float min = float.MinValue, float max = float.MaxValue) : base(owner) {
			Base.Value = Current.Value = @base; Min.Value = min; Max.Value = max; Mult.Value = 1;
		}

		protected override float GetTotal() {
			var t = (Current.Value + Add.Value) * Mult.Value;
			return Mathf.Clamp(t, Min.Value, Max.Value);
		}
	}

	[Serializable]
	public class StatSemiInt : StatFloat {
		Func<float, int> round;
		new public int Total => round(base.Total);

		public StatSemiInt(object owner, int @base = 0, int min = int.MinValue, int max = int.MaxValue, Func<float, int> round = null)
			: base(owner, @base, min, max) => this.round = round ?? Mathf.RoundToInt;

		public StatSemiInt(int @base = 0, int min = int.MinValue, int max = int.MaxValue, Func<float, int> round = null)
			: base(@base, min, max) => this.round = round ?? Mathf.RoundToInt;
	}

}
