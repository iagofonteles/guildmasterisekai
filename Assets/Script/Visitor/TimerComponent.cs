using UnityEngine;
using UnityEngine.Events;

namespace GuildMasterIsekai.Visitor {
	public class TimerComponent : MonoBehaviour {
		[SerializeField, Min(.01f)] private float interval = 10f;
		[SerializeField] private UnityEvent onTrigger = new();
		private float remaining;
		public UnityEvent OnTrigger => onTrigger;

		private void OnEnable() => remaining = Mathf.Max(.01f, interval);

		private void Update() {
			remaining -= Time.deltaTime;
			if(remaining > 0) return;
			remaining = Mathf.Max(.01f, interval);
			onTrigger.Invoke();
		}
	}
}
