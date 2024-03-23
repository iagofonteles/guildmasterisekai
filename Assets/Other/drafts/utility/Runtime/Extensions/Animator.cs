using System;
using System.Collections;
using UnityEngine;

namespace Drafts.Extensions {
	public static class ExtensionsAnimator {

		/// <summary>Get the length of current clip playing.</summary>
		public static float Duration(this Animator a, int layer = 0) => a.GetCurrentAnimatorStateInfo(layer).length;

		public static IEnumerator WaitAnimation(this Animator anim, string state, int layer = 0, float scale = 1) {
			anim.Play(state, layer);
			yield return new WaitForEndOfFrame();
			var time = scale * anim.Duration(layer);

			switch(anim.updateMode) {
				case AnimatorUpdateMode.Normal: yield return new WaitForSeconds(time); break;
				case AnimatorUpdateMode.AnimatePhysics: yield return new WaitForSeconds(time); break;
				case AnimatorUpdateMode.UnscaledTime: yield return new WaitForSecondsRealtime(time); break;
			}
			yield return new WaitForEndOfFrame();
		}

		public static IEnumerator WaitTransition(this Animator anim, int layer = 0, float scale = 1) {
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			var state = anim.GetCurrentAnimatorStateInfo(layer);
			Debug.Log(state.IsName("Close"));

			yield return new WaitUntil(() => {
				Debug.Log(state.normalizedTime);
				return (state.normalizedTime % 1) == 0;
			});
			yield return new WaitForEndOfFrame();
		}

		public static void Play(this Animator anim, string state, Action callback) => Play(anim, state, 0, 1, callback);
		public static void Play(this Animator anim, string state, int layer, float scale, Action callback)
			=> _Play(anim, state, layer, scale, callback).Start();
		static IEnumerator _Play(Animator anim, string state, int layer, float scale, Action callback) {
			if(!anim || !anim.isActiveAndEnabled) yield break;
			yield return anim.WaitAnimation(state, layer, scale);
			callback?.Invoke();
		}

		public static IEnumerator WaitTransition(this Animator anim, string parameter, bool value, int layer = 0, float scale = 1) {
			anim.SetBool(parameter, value);
			yield return WaitTransition(anim, layer);
		}

		public static void SetLinear(this AnimationCurve curve, int index1, int index2) {
			var a = curve.keys[index1];
			var b = curve.keys[index2];
			var delta = new Vector2(b.time, b.value) - new Vector2(a.time, a.value);
			a.outTangent = delta.y / delta.x;
			b.inTangent = delta.y / delta.x;
			curve.MoveKey(index1, a);
			curve.MoveKey(index2, b);
		}

		//public static void SetLinear(this AnimationCurve curve) { for (var i = 1; i < curve.keys.Length; i++) SetLinear(curve, i - 1, i); }

		public static void SetLinear(this AnimationCurve curve) {
			float inTangent, outTangent;
			Vector2 delta;
			Keyframe key;
			Vector2 Delta(Keyframe a, Keyframe b) => new Vector2(b.time, b.value) - new Vector2(a.time, a.value);

			for(int i = 0; i < curve.keys.Length; ++i) {
				key = curve.keys[i];

				if(i == 0) inTangent = 0;
				else {
					delta = Delta(curve.keys[i - 1], key);
					inTangent = delta.y / delta.x;
				}
				if(i == curve.keys.Length - 1) outTangent = 0;
				else {
					delta = Delta(key, curve.keys[i + 1]);
					outTangent = delta.y / delta.x;
				}

				key.inTangent = inTangent;
				key.outTangent = outTangent;
				curve.MoveKey(i, key);
			}
		}
	}
}