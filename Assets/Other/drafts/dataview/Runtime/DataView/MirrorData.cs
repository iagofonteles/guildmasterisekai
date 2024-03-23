using UnityEngine;
namespace Drafts {
	public class MirrorData : MonoBehaviour {
		[SerializeField] DataHolder source;
		[SerializeField] DataHolder target;
		private void Awake() {
			if(!target) target = GetComponent<DataHolder>();
			OnDataChanged(source.Data);
			source.OnDataChanged.AddListener(OnDataChanged);
		}
		private void OnDestroy() => source.OnDataChanged.RemoveListener(OnDataChanged);
		private void OnDataChanged(object data) => target.Data = data;
	}
}