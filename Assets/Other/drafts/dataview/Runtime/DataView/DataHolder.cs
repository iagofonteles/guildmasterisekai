using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Drafts {

	public class DataHolder : MonoBehaviour, IData, IPointerClickHandler {
		public UnityEvent<object> OnDataChanged;

		protected object data;
		public object Data { get => data; set => SetData(value); }

		protected virtual void SetData(object value) {
			data = value is IData d ? d.Data : value;
			OnDataChanged?.Invoke(data);
		}

		public void CopyFrom(UnityEngine.Object value) => Data = value switch {
			IData idata => idata.Data,
			GameObject go => go.GetComponent<IData>()?.Data,
			_ => value
		};

		public void ResetData() => SetData((object)null);
		public void DestroyTarget(GameObject obj) => Destroy(obj);
		public void DestroySelf() => Destroy(gameObject);
		public void Log(string message) => Debug.Log(message, this);

		public IEnumerator Wait => new WaitWhile(() => this && isActiveAndEnabled);
		public IEnumerator WaitDestroy => new WaitWhile(() => this);

		public static event Action<DataHolder, PointerEventData> OnClicked;
		void IPointerClickHandler.OnPointerClick(PointerEventData eventData) => OnClicked?.Invoke(this, eventData);
	}
}
