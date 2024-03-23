//using System;
//using UnityEngine;
//namespace Drafts.Patterns {

//	/// <summary>
//	/// Triggers callbacks on value change containing the delta and source of modification
//	/// Respect Min and Max values
//	/// Has a Base value so you can check if value was modified
//	/// </summary>
//	[Serializable]
//	public class NumStatWatcher<VAL, OWN, T, SRC> : StatWatcher<VAL, OWN, T, SRC>, INumStatWatcher<VAL> where T : NumStatWatcher<VAL, OWN, T, SRC> {
//		public delegate void Processor(T stat, SRC source, int current, ref int delta);
//		[SerializeField] VAL min, max, @base, value;

//		public VAL Min => min;
//		public VAL Max => max;

//		public event Callback OnMinChanged;
//		public event Callback OnMaxChanged;
//		public event Processor Processors;

//		public NumStatWatcher() { }
//		public NumStatWatcher(int value) : this(default, value) { }
//		public NumStatWatcher(OWN owner, int value) {
//			this.owner = owner;
//			this.value = value;
//			@base = value;
//		}

//		public void Add(SRC source, int value) => Set(source, this.value + value);
//		public void Sub(SRC source, int value) => Set(source, this.value - value);
//		public void Mult(SRC source, int value) => Set(source, this.value * value);
//		public void Div(SRC source, int value) => Set(source, this.value / value);
//		public void Reset(SRC source) => Set(source, @base);

//		public void ChangeMin(SRC source, VAL value) {
//			var delta = GetDelta(min, value);
//			if(delta != 0) {
//				min = value;
//				Set(source, this.value);
//				OnMinChanged?.Invoke((T)this, source, value, delta);
//				_OnMinChanged?.Invoke(value);
//			}
//		}
//		public void ChangeMax(SRC source, int value) {
//			var delta = value - max;
//			if(delta != 0) {
//				max = value;
//				Set(source, this.value);
//				OnMaxChanged?.Invoke((T)this, source, value, delta);
//				_OnMaxChanged?.Invoke(value);
//			}
//		}
//		public void ChangeBase(SRC source, int value) {
//			value = Math.Clamp(value, min, max);
//			var delta = value - @base;
//			if(delta != 0) {
//				@base = value;
//				OnBaseChanged?.Invoke((T)this, source, value, delta);
//				_OnBaseChanged?.Invoke(value);
//			}
//		}

//		public override string ToString() => $"val/base: {value}/{@base}, min/max: {min}/{max}";
//		public static implicit operator int(NumStatWatcher<SRC, OWN, T> s) => s.value;


//		#region Interface
//		event Action<int> _OnBaseChanged;
//		event Action<int> _OnMinChanged;
//		event Action<int> _OnMaxChanged;

//		event Action<int> IStatWatcher<int>.OnBaseChanged {
//			add => _OnBaseChanged += value;
//			remove => _OnBaseChanged -= value;
//		}
//		event Action<int> IStatWatcher<int>.OnMinChanged {
//			add => _OnMinChanged += value;
//			remove => _OnMinChanged -= value;
//		}
//		event Action<int> IStatWatcher<int>.OnMaxChanged {
//			add => _OnMaxChanged += value;
//			remove => _OnMaxChanged -= value;
//		}
//		#endregion
//	}
//}