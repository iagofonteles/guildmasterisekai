using UnityEngine;
using UnityEngine.Events;

namespace Drafts {

	[AddComponentMenu("Drafts/Components/Timer")]
	public class TimerComponent : MonoBehaviour {
		[Min(0)] public float timeLimit = 1;
		public bool autoReset = true;
		public bool playOnStart = true;

		public UnityEvent OnTrigger = new();
		public UnityEvent<float> OnUpdate = new();
		public UnityEvent OnReset = new();

		Timer timer;

		public bool Pause { get => timer.pause; set => timer.pause = value; }

		private void Awake() {
			timer = new Timer(autoReset) {
				onTrigger = OnTrigger.Invoke,
				onUpdate = OnUpdate.Invoke,
				onReset = OnReset.Invoke,
			};
			if(!playOnStart) timer.pause = true;
		}

		void Update() => timer?.Update(timeLimit);
		public void Play() => timer?.Reset(true);
		public void Trigger() => timer?.Trigger();
		public static implicit operator Timer(TimerComponent t) => t.timer;
	}
}
