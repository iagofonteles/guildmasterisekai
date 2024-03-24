using System;
using UnityEngine;

namespace Drafts {
	/// <summary>Based on DateTime. Call Update to triggger events. Good to sync UI with cooldowns.
	/// Cannot be paused.</summary>
	[Serializable]
	public class TimerClock : Timer {
		[SerializeField] DateTime time = DateTime.Now;
		/// <summary>Current elapsed time.</summary>
		public TimerClock(bool autoReset = true, Action onTrigger = null) : base(autoReset, onTrigger) { }
		/// <summary>Based on DateTime. 0 means Now.</summary>
		public override float Time { get => (float)(DateTime.Now - time).TotalSeconds; set => time = DateTime.Now.AddSeconds(value); }

		protected override bool _update(float timeLimit, float deltaTime) {
			if(_triggered || pause) return false;
			var amt = Time / timeLimit;
			if(amt < 1) {
				onUpdate?.Invoke(amt);
				return false;
			} else {
				Trigger();
				return true;
			}
		}

	}
}
