using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Drafts.UI {

	[RequireComponent(typeof(CanvasGroup))]
	public class DraggableItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {

		public bool centralize;
		public static event Action<PointerEventData> OnDraggingItem;
		RectTransform rect;
		CanvasGroup group;
		Vector2 origin;

		[Serializable]
		public class Callbacks {
			public UnityEvent<PointerEventData> OnDragBegin;
			public UnityEvent<PointerEventData> OnDrag;
			public UnityEvent<PointerEventData> OnDragEnd;
		}
		public Callbacks callbacks;

		private void Awake() {
			rect = GetComponent<RectTransform>();
			group = GetComponent<CanvasGroup>();
		}

		public void OnBeginDrag(PointerEventData eventData) {
			group.blocksRaycasts = false;
			origin = rect.anchoredPosition;
			callbacks.OnDragBegin.Invoke(eventData);
			OnDraggingItem?.Invoke(eventData);
		}

		public void OnEndDrag(PointerEventData eventData) {
			group.blocksRaycasts = true;
			rect.anchoredPosition = origin;
			callbacks.OnDragEnd.Invoke(eventData);
			eventData.pointerDrag = null;
			OnDraggingItem?.Invoke(eventData);
		}

		public void OnDrag(PointerEventData eventData) {
			if(centralize) rect.position = eventData.position;
			else rect.anchoredPosition += eventData.delta;
			callbacks.OnDrag.Invoke(eventData);
		}

		public static void CancelDrag(PointerEventData eventData) {
			var item = eventData.pointerDrag?.GetComponent<DraggableItem>();
			eventData.pointerDrag = null;
			item?.OnEndDrag(eventData);
		}
	}
}
