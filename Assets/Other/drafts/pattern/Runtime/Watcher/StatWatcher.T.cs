//using System;
//using System.Collections.Generic;
//using UnityEngine;
//namespace Drafts.Patterns {

//	/// <summary>
//	/// Triggers callbacks on value change containing the delta and source of modification
//	/// Respect Min and Max values
//	/// Has a Base value so you can check if value was modified
//	/// </summary>
//	[Serializable]
//	public class StatWatcher<VAL> : IWatcher<VAL>
//#if DRAFTS_UTILITY
//		,IOnChanged<VAL>
//#endif
//		{

//		[SerializeField] VAL @base, value;
//		public object owner;
//		public VAL Base => @base;
//		public VAL Value => value;

//		public event Action<VAL> OnChanged;
//		public event ChangedEventHandler<VAL> OnBaseChanged;
//		public event ChangedEventHandler<VAL> OnValueChanged;

//		public StatWatcher() { }
//		public StatWatcher(VAL value) : this(default, value) { }
//		public StatWatcher(object owner, VAL value) {
//			this.owner = owner;
//			this.value = value;
//		}

//		protected virtual VAL GetDelta(VAL o, VAL n) => o;

//		public virtual void Set(object source, VAL value) {
//			if(EqualityComparer<VAL>.Default.Equals(value, this.value)) return;
//			var delta = GetDelta(this.value, value);
//			this.value = value;
//			OnChanged?.Invoke(value);
//			OnValueChanged?.Invoke(this,new( source, value, delta));
//		}
//		public void Reset(object source) => Set(source, @base);

//		public virtual void ChangeBase(object source, VAL value) {
//			if(EqualityComparer<VAL>.Default.Equals(value, @base)) return;
//			var delta = GetDelta(@base, value);
//			@base = value;
//			OnChanged?.Invoke(value);
//			OnValueChanged?.Invoke(this, new(source, value, delta));
//		}

//		public override string ToString() => $"val/base: {value}/{@base}";
//	}

//}