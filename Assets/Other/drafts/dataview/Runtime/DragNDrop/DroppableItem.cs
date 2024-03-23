using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Drafts.UI {

	public interface IDroppableFilter {
		bool Match(GameObject go);

		public class None : IDroppableFilter {
			public bool Match(GameObject go) => go;
		}
	}

	public class DroppableItem : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

		[SerializeReference, TypeInstance] public IDroppableFilter filter = new IDroppableFilter.None();
		public UnityEvent<PointerEventData> OnItemDropped;

		[Serializable]
		public class Callbacks {
			public UnityEvent<PointerEventData> OnEnter;
			public UnityEvent<PointerEventData> OnExit;
			public UnityEvent<PointerEventData> OnEnabled;
			public UnityEvent<PointerEventData> OnDisabled;
		}
		public Callbacks callbacks;

		private void OnEnable() => DraggableItem.OnDraggingItem += OnDraggingItem;
		private void OnDisable() => DraggableItem.OnDraggingItem -= OnDraggingItem;

		private void OnDraggingItem(PointerEventData eventData) {
			if(filter.Match(eventData.pointerDrag)) callbacks.OnEnabled.Invoke(eventData);
			else callbacks.OnDisabled.Invoke(eventData);
		}

		public void OnDrop(PointerEventData eventData) {
			if(filter.Match(eventData.pointerDrag))
				OnItemDropped.Invoke(eventData);
			callbacks.OnExit.Invoke(eventData);
		}

		public void OnPointerEnter(PointerEventData eventData) {
			if(filter.Match(eventData.pointerDrag))
				callbacks.OnEnter.Invoke(eventData);
		}

		public void OnPointerExit(PointerEventData eventData) {
			callbacks.OnExit.Invoke(eventData);
		}
	}
}
