using System;

namespace Drafts {
	/// <summary>Based on Time.deltatime. Trigger events as time passes. Good to sync UI with cooldowns.</summary>
	[Serializable]
	public class Timer {
		/// <summary>Current elapsed time.</summary>
		public virtual float Time { get; set; } = 0;
		/// <summary>Auto reset after tirgger.</summary>
		public bool autoReset = true;
		/// <summary>Pause and Resume timer.</summary>
		public bool pause = false;

		/// <summary>Called once reach timeLimit.</summary>
		public Action onTrigger;
		/// <summary>Called on reset.</summary>
		public Action onReset;
		/// <summary>Called on Update</summary>
		public Action<float> onUpdate;

		/// <summary>Overrides WaitWhile and FixedTimeLimit.</summary>
		public float TimeLimit { get => GetTimeLimit(); set => GetTimeLimit = () => value; }
		/// <summary>Overrides TimeLimit and FixedTimeLimit.</summary>
		public Func<bool> WaitWhile { set => GetTimeLimit = () => value() ? float.MaxValue : 0; }
		/// <summary>Overrides WaitWhile and TimeLimit.</summary>
		public Func<float> GetTimeLimit { get; set; } = () => 0;

		protected bool _triggered = false;

		public Timer() { }
		public Timer(bool autoReset = true, Action onTrigger = null) { this.autoReset = autoReset; this.onTrigger = onTrigger; }

		/// <summary>Not triggered AND not paused.</summary>
		public bool IsPlaying => !_triggered && !pause;
		/// <summary>Already triggered.</summary>
		public bool IsDone => _triggered;
		/// <summary>Pause and Resume timer. Note: you still need to call Update().</summary>
		public void Pause(bool b) => pause = b;
		/// <summary>Set time to 0 and call OnReset.</summary>
		public virtual void Reset(bool resume = false) { _triggered = false; Time = 0; onReset?.Invoke(); if(resume) pause = false; }

		/// <summary>Same as Update(float). Return true when Triggered.</summary>
		public bool this[float timeLimit] => _update(timeLimit, UnityEngine.Time.deltaTime);
		public bool this[float timeLimit, float deltaTime] => _update(timeLimit, deltaTime);

		/// <summary>Return true when triggered.</summary>
		public void Update() => _update(UnityEngine.Time.deltaTime);
		/// <summary>Return true when triggered.</summary>
		public void Update(float timeLimit) => _update(timeLimit, UnityEngine.Time.deltaTime);

		/// <summary>Return true when triggered.</summary>
		public bool Ready() { _update(GetTimeLimit(), UnityEngine.Time.deltaTime); return _triggered; }
		/// <summary>Return true when triggered.</summary>
		public bool ReadyUnscaled() => _update(GetTimeLimit(), UnityEngine.Time.unscaledDeltaTime);

		/// <summary>Return true when triggered.</summary>
		public void FixedUpdate() => _update(UnityEngine.Time.fixedDeltaTime);
		/// <summary>Return true when triggered.</summary>
		public void FixedUpdate(float timeLimit) => _update(timeLimit, UnityEngine.Time.fixedDeltaTime);

		protected bool _update(float deltaTime) => _update(GetTimeLimit(), deltaTime);
		protected virtual bool _update(float timeLimit, float deltaTime) {
			if(_triggered || pause) return false;
			var amt = (Time += deltaTime) / timeLimit;
			if(amt < 1) {
				onUpdate?.Invoke(amt);
				return false;
			} else {
				Time -= timeLimit;
				onUpdate?.Invoke(1);
				onTrigger?.Invoke();
				_triggered = !autoReset;
				if(autoReset) onReset?.Invoke();
				return true;
			}
		}

		public void Trigger() {
			onUpdate?.Invoke(1);
			onTrigger?.Invoke();
			_triggered = true;
			if(autoReset) Reset();
		}
		public void Cancel() {
			onUpdate?.Invoke(0);
			_triggered = true;
		}

		public static explicit operator Timer(float time) => new Timer(false) { TimeLimit = time };
		public static implicit operator Func<bool>(Timer timer) => () => timer.Ready();
		public static implicit operator bool(Timer timer) => timer.Ready();
	}
}
