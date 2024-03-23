using System.Linq;
using UnityEngine;
namespace Drafts {

	public class CanvasSorter : MonoBehaviour {
		[SerializeField] int colunms = 0;

		[ContextMenu("Sort")]
		void Sort() {
			var pages = gameObject.GetComponentsInChildren<ICanvasSortable>();
			pages = pages.Where(p => p.transform.GetComponentInParent<CanvasSorter>() == this).ToArray();
			var f = TryGetComponent<Canvas>(out var c) ? c.scaleFactor : 1;
			var rt = transform as RectTransform;
			var rect = rt.rect;
			var root = colunms > 0 ? colunms : Mathf.CeilToInt(Mathf.Sqrt(pages.Length));

			var offset = 0;
			for(int i = 0; i < pages.Length; i++) {
				var x = offset % root;
				var y = offset / root;

				pages[i].transform.position = rect.position + new Vector2(rect.width * x, -rect.height * y) * f;
				offset += pages[i].Size;
			}
		}

		[ContextMenu("Restore")]
		void Restore() {
			var pages = gameObject.GetComponentsInChildren<ICanvasSortable>(true);
			foreach(var p in pages) p.transform.localPosition = Vector3.zero;
		}
	}

}
