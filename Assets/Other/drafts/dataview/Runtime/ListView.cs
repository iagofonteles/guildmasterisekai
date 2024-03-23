using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Drafts {
	/// <summary>
	/// Currently the anchors of the template should be min .5, 1 and max .5, 1.
	/// Transform scale does not account for the item height.
	/// </summary>
	public class ListView : MonoBehaviour {

		[SerializeField] RectTransform template;
		[SerializeField] bool horizontal = false;
		[SerializeField] float spacing;
		[SerializeField] float sensitivity = .2f;
		[SerializeField] Vector2 clampScroll;

		Canvas canvas;
		int dir;
		float itemSize, scroll;
		Vector2 origin, direction;
		Vector2 clamp, bounds;
		List<RectIndex> itens = new();

		IList data;
		public IList Data {
			get => data;
			set {
				data = value;
				clamp.y = (data?.Count ?? 0) * itemSize + itemSize * (clampScroll.y - 1);
				foreach(var item in itens) item.index = -1;
				GoToTop();
			}
		}

		public float Scroll {
			get => scroll;
			set {
				scroll = value = Mathf.Clamp(value, clamp.x, clamp.y);
				for(int i = 0; i < itens.Count; i++) {
					var amount = origin[dir] - value + itemSize * i;
					amount = Loop(amount, bounds.x, bounds.y, out var times);
					itens[i].rect.anchoredPosition = direction * amount;
					SetItemIndex(itens[i], i - times * itens.Count);
				}
			}
		}

		void Awake() {
			canvas = GetComponentInParent<Canvas>();
			origin = template.anchoredPosition;
			dir = horizontal ? 0 : 1;
			direction = horizontal ? Vector3.right : Vector3.down;
			itemSize = template.sizeDelta[dir] + spacing;
			CreateItens();
		}

		void CreateItens() {
			var container = template.parent.GetComponent<RectTransform>();
			var itemCount = 2 + Mathf.CeilToInt(container.sizeDelta[dir] / itemSize);

			itens.Add(new(template));
			for(int i = container.childCount; i < itemCount; i++)
				itens.Add(new(Instantiate(template, container)));

			bounds.x = container.rect.min[dir] - itemSize;
			bounds.y = container.rect.max[dir] + itemSize;
			clamp.x = -itemSize * clampScroll.x;
			clamp.y = itemSize * (clampScroll.y - 1);

			GoToTop();
		}

		[ContextMenu("GoToTop")]
		public void GoToTop() => Scroll = 0;
		public void AddScroll(float delta) => Scroll += delta;
		public void AddScroll(Vector2 delta) => Scroll += delta[dir];

		void SetItemIndex(RectIndex item, int index) {
			if(item.index == index) return;
			item.index = index;
			item.iindex?.SetIndex(index);
			item.idata?.SetData(index >= 0 && index < Data?.Count
				? Data[index] : null);
		}

		//void IDragHandler.OnDrag(PointerEventData eventData) {
		//	var amt = sensitivity * (horizontal ? eventData.delta.x : eventData.delta.y) / canvas.scaleFactor;
		//	Scroll -= amt;
		//}

		class RectIndex {
			public RectTransform rect;
			public IData idata;
			public IIndex iindex;
			public int index = -1;

			public RectIndex(RectTransform rect) {
				this.rect = rect;
				idata = rect.GetComponent<IData>();
				iindex = rect.GetComponent<IIndex>();
			}
		}

		static float Loop(float n, float min, float max, out int times) {
			float range = max - min;
			times = Mathf.FloorToInt((n - min) / range);
			return n - times * range;
		}
	}
}