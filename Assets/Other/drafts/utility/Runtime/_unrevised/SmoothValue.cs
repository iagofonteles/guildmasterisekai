using System;
using System.Collections;
using UnityEngine;
namespace Drafts {

	/// <summary>
	/// Set a Speed or Duration for the animation
	/// Optionally set the Updater, if it is disabled, the animation will pause until enabled again
	/// </summary>
	public class SmoothValue {
		//[Flags] public enum UpdateMode { Local = 0, Global = 1, Unscaled = 2, GlobalUnscaled = 3 }

		public event Action<float> OnChanged;
		public event Action<float> OnAnimate;
		public event Action<float> OnAnimationEnd;
		public float Speed { set => getMaxDelta = (a, b) => value; }
		public float Duration { set => getMaxDelta = (a, b) => Mathf.Abs((b - a) / value); }
		public MonoBehaviour Updater { get; set; }

		float value;
		Coroutine routine;
		Func<float, float, float> getMaxDelta = (a, b) => 1;

		public SmoothValue(Action<float> onAnimate) : this(0, null, onAnimate) { }
		public SmoothValue(float value = 0, MonoBehaviour updater = null, Action<float> onAnimate = null) {
			this.value = value;
			OnAnimate = onAnimate;
			Updater = updater ?? Drafts.Updater.Instance;
		}

		public float Current { get; private set; }
		public float Value {
			get => value;
			set {
				if(!Updater) return;
				this.value = value;
				OnChanged?.Invoke(value);
				if(routine != null) Updater.StopCoroutine(routine);
				routine = Updater.StartCoroutine(SetLinear(value));
			}
		}

		IEnumerator SetLinear(float target) {
			var delta = getMaxDelta(Current, target);
			while(Current != target) {
				Current = Mathf.MoveTowards(Current, target, delta * Time.deltaTime);
				OnAnimate?.Invoke(Current);
				yield return null;
			}
			OnAnimationEnd?.Invoke(target);
		}

		static IEnumerator SetLinearNormalized(float current, float target, float duration, Action<float> setValue) {
			var delta = target - current;
			var speed = delta / duration;
			while(current != target) {
				current = Mathf.MoveTowards(current, target, speed * Time.deltaTime);
				setValue(current);
				yield return null;
			}
			setValue(target);
		}

		public static void Normalized(float start, float end, float duration, Action<float> setValue)
			=> Drafts.Updater.Instance.StartCoroutine(SetLinearNormalized(start, end, duration, setValue));
	}
}
