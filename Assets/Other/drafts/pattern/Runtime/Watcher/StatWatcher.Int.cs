//using System;
//using UnityEngine;
//namespace Drafts.Patterns {

//	/// <summary>
//	/// Triggers callbacks on value change containing the delta and source of modification
//	/// Respect Min and Max values
//	/// Has a Base value so you can check if value was modified
//	/// </summary>
//	[Serializable]
//	public class IntStatWatcher : INumStatWatcher<int>
//#if DRAFTS_UTILITY
//		,IOnChanged<int>
//#endif
//		{

//		[SerializeField] int min = int.MinValue;
//		[SerializeField] int max = int.MaxValue;
//		[SerializeField] int @base, value;
//		public object owner;

//		public int Min => min;
//		public int Max => max;
//		public int Base => @base;
//		public int Value => value;

//		public event Action<int> OnChanged;
//		public event ChangedEventHandler<int> OnMinChanged;
//		public event ChangedEventHandler<int> OnMaxChanged;
//		public event ChangedEventHandler<int> OnBaseChanged;
//		public event ChangedEventHandler<int> OnValueChanged;
//		public event ChangedEventHandler<int> Processors;

//		public IntStatWatcher() { }
//		public IntStatWatcher(int value) : this(default, value) { }
//		public IntStatWatcher(object owner, int value) : this(owner, value, int.MinValue, int.MaxValue) { }
//		public IntStatWatcher(object owner, int value, int min, int max) {
//			this.owner = owner;
//			this.value = value;
//			this.min = min;
//			this.max = max;
//			@base = value;
//		}

//		public void Set(object source, int value) {
//			value = Math.Clamp(value, min, max);
//			var args = new ChangedEventArgs<int>(source, value, this.value);
//			Processors?.Invoke(this, args);
//			value = args.New;

//			if(this.value == value) return;
//			var old = this.value;
//			this.value = value;
//			OnChanged?.Invoke(this.value);
//			OnValueChanged?.Invoke(this, new(source, this.value, old));
//		}
//		public void Add(object source, int value) => Set(source, this.value + value);
//		public void Sub(object source, int value) => Set(source, this.value - value);
//		public void Mult(object source, int value) => Set(source, this.value * value);
//		public void Div(object source, int value) => Set(source, this.value / value);
//		public void Reset(object source) => Set(source, @base);
//		public void Empty(object source) => Set(source, min);
//		public void Fill(object source) => Set(source, max);

//		public void ChangeMin(object source, int value) {
//			if(min == value) return;
//			var old = min;
//			min = value;
//			Set(source, this.value);
//			OnChanged?.Invoke(value);
//			OnMinChanged?.Invoke(this, new(source, value, old));
//		}
//		public void ChangeMax(object source, int value) {
//			if(max == value) return;
//			var old = min;
//			max = value;
//			Set(source, this.value);
//			OnMaxChanged?.Invoke(this, new(source, value, old));
//		}
//		public void ChangeBase(object source, int value) {
//			value = Math.Clamp(value, min, max);
//			if(@base == value) return;
//			var old = @base;
//			@base = value;
//			OnBaseChanged?.Invoke(this, new(source, value, old));
//		}

//		public override string ToString() => $"val/base: {value}/{@base}, min/max: {min}/{max}";
//	}
//}
