using System;
using UnityEngine;
using UnityEngine.Events;
namespace Drafts {

	public partial class Anima : MonoBehaviour {
		[Min(.001f)] public float duration = .25f;
		public bool unscaledTime;
		[SerializeField, Range(0, 1)] float progress = 1;
		public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
		public TypeInstance<IAnim> modules;

		[Serializable]
		public class Callbacks {
			public UnityEvent<bool> OnPlay;
			public UnityEvent<float> OnUpdate;
			public UnityEvent<bool> OnFinish;
			public UnityEvent OnOpening;
			public UnityEvent OnOpened;
			public UnityEvent OnClosing;
			public UnityEvent OnClosed;
		}
		public Callbacks callbacks;

		public float Progress => progress;
		public float Direction { get; private set; }

		void Awake() {
			Direction = progress < .5f ? 1 : -1;
			foreach(var module in modules) module.Subscribe(this);
		}

		void Update() {
			var delta = unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
			delta = delta / duration * Direction;
			progress += delta;

			if(progress <= 0) {
				progress = 0;
				enabled = false;
				callbacks.OnFinish?.Invoke(false);
				callbacks.OnClosed?.Invoke();
			}
			if(progress >= 1) {
				progress = 1;
				enabled = false;
				callbacks.OnFinish?.Invoke(true);
				callbacks.OnOpened?.Invoke();
			}
			OnValidate();
		}

		#region Play

		public void Open() => Play();
		public void Close() => PlayBackwards();

		public void Play() {
			Direction = 1;
			enabled = true;
			callbacks.OnPlay.Invoke(true);
			callbacks.OnOpening.Invoke();
		}

		public void PlayBackwards() {
			Direction = -1;
			enabled = true;
			callbacks.OnPlay.Invoke(false);
			callbacks.OnClosing.Invoke();
		}

		public void Play(bool forward) {
			if(forward) Play();
			else PlayBackwards();
		}

		public void SetProgress(float progress) {
			this.progress = Mathf.Clamp01(progress);
			Direction = progress < .5f ? 1 : -1;
			OnValidate();
		}

		public void Restart(bool forward) {
			SetProgress(forward ? 0 : 1);
			Play(forward);
		}

		public void Toggle() {
			if(Direction < 0) Play();
			else PlayBackwards();
		}

		#endregion

		private void OnValidate() {
			if(!enabled) return;
			var animated = curve.Evaluate(progress);
			foreach(var module in modules) module.Update(animated);
			callbacks.OnUpdate.Invoke(animated);
		}

	}
}
