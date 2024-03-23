//using System;
//using UnityEngine;
//namespace Drafts.Patterns {

//	/// <summary>
//	/// Triggers callbacks on value change containing the delta and source of modification
//	/// Respect Min and Max values
//	/// Has a Base value so you can check if value was modified
//	/// </summary>
//	[Serializable]
//	public class FloatStatWatcher : IWatcher<float>, IStatWatcher<float> {

//		public delegate void RangeChangedHandler(object source, RangeChangedArgs args);
//		public class RangeChangedArgs {
//			public object Source;
//			public float Min, Max;
//			public RangeChangedArgs(object source, float min, float max) { Source = source; Min = min; Max = max; }
//		}

//		public object owner;
//		[SerializeField] float value, @base, add, mult = 1;
//		[SerializeField] float min = float.MinValue;
//		[SerializeField] float max = float.MaxValue;

//		public class Buffs {
//			public float add;
//			public float mult;
//		}

//		public float IntValue => Mathf.RoundToInt(Value);
//		public float Value => value;
//		public float Base => @base;
//		public float Min => min;
//		public float Max => max;

//		public event Action<float> OnChanged;
//		public event RangeChangedHandler OnRangeChanged;
//		public event ChangedEventHandler<float> OnStatChanged;

//		public FloatStatWatcher() { }
//		public FloatStatWatcher(float value) : this(default, value, float.MinValue, float.MaxValue) { }
//		public FloatStatWatcher(object owner, float @base) : this(owner, @base, float.MinValue, float.MaxValue) { }
//		public FloatStatWatcher(object owner, float @base, float min, float max) {
//			this.owner = owner;
//			value = @base;
//			this.min = min;
//			this.max = max;
//			this.@base = @base;
//		}

//		public void Set(object source, float value) {
//			value = ;
//			value = Math.Clamp(Evaluate(value), min, max);
//			var args = new ChangedEventArgs<float>(source, value, this.value);
//			value = args.New;

//			if(this.value == value) return;
//			var old = this.value;
//			this.value = value;
//			OnChanged?.Invoke(this.value);
//			OnValueChanged?.Invoke(this, new(source, this.value, old));
//		}
//		public void Add(object source, float value) => Set(source, this.value + value);
//		public void Sub(object source, float value) => Set(source, this.value - value);
//		public void Mult(object source, float value) => Set(source, this.value * value);
//		public void Div(object source, float value) => Set(source, this.value / value);

//		public void BonusAdd(object source, float value) { add += value; Set(source, value); }
//		public void BonusMult(object source, float value) { mult += value; Set(source, value); }

//		public void Reset(object source) => Set(source, @base);
//		public void Empty(object source) => Set(source, min);
//		public void Fill(object source) => Set(source, max);

//		float Evaluate() => (value + add) * mult;

//		public void ChangeMin(object source, float value) {
//			if(min == value) return;
//			var old = min;
//			min = value;
//			Set(source, this.value);
//			OnChanged?.Invoke(value);

//			OnRangeChanged.Invoke(this, new(source, min, max));
//		}

//		public void ChangeMax(object source, float value) {
//			if(max == value) return;
//			var old = min;
//			max = value;
//			Set(source, this.value);
//			OnRangeChanged.Invoke(this, new(source, min, max));
//		}

//		public void ChangeBase(object source, float value) {
//			value = Math.Clamp(value, min, max);
//			if(@base == value) return;
//			@base = value;
//			Set(source, Value);
//		}

//		public override string ToString() => $"val/base: {value}/{@base}, min/max: {min}/{max}";
//	}
//}