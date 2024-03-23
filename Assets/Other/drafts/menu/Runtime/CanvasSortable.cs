using UnityEngine;

namespace Drafts {

	public interface ICanvasSortable {
		Transform transform { get; }
		int Size { get; }
	}

	public class CanvasSortable : MonoBehaviour, ICanvasSortable {
		[SerializeField] int size = 1;
		[SerializeField] bool deactivateOnAwake;
		public int Size => size;
		private void Awake() {
			var rect = GetComponent<RectTransform>();
			rect.anchoredPosition = Vector3.zero;
			if(deactivateOnAwake) gameObject.SetActive(false);
		}
	}

}
