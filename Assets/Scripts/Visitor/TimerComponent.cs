using UnityEngine;
using UnityEngine.Events;

namespace GuildMasterIsekai.Visitor
{
    public class TimerComponent : MonoBehaviour
    {
        [SerializeField, Min(.01f)] private float interval = 10f;
        [SerializeField] private UnityEvent onTrigger = new();
        public UnityEvent OnTrigger => onTrigger;
        private float _remaining;

        private void Update()
        {
            _remaining -= Time.deltaTime;
            if (_remaining > 0) return;
            _remaining = Mathf.Max(.01f, interval);
            onTrigger.Invoke();
        }

        private void OnEnable() => _remaining = Mathf.Max(.01f, interval);
    }
}