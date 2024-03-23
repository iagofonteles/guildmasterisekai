//using Drafts.Extensions;
//using System;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Drafts {

//	public static class ExtensionsDataViewSlider {
//		public static DataViewSlider<T> GetDataViewSlider<T>(this DataView<T> view, float speed = .15f, bool vertical = false)
//			 => new DataViewSlider<T>(view, speed, vertical);

//		public static DataSlider<V, D> GetDataSlider<V, D>(this V view, List<D> data, float speed = .15f, bool vertical = false)
//			where V : MonoBehaviour, IData<D> => new DataSlider<V, D>(view, speed, vertical);
//	}

//	public class DataViewSlider<T> : DataSlider<DataView<T>, T> {
//		public DataViewSlider(DataView<T> view, float speed = 0.15F, bool vertical = false) : base(view, speed, vertical) { }
//	}

//	public class DataSlider<V, D> : IData<List<D>> where V : MonoBehaviour, IData<D> {

//		readonly V view;
//		readonly RectTransform rect;
//		readonly CanvasGroup canvas;
//		readonly Action<float> moveOut;
//		readonly Action<float> moveIn;
//		readonly bool vertical;

//		Coroutine coroutine;
//		List<D> data = new List<D>();
//		int index;
//		float size;
//		float speed;

//		public List<D> Data { get => data; set { data = value; GoTo(0); } }
//		public D Current => data[index];

//		public Action<int> OnIndexChanged;

//		public DataSlider(V view, float speed = .15f, bool vertical = false) {
//			this.view = view;
//			this.speed = speed;
//			this.vertical = vertical;

//			rect = view.GetComponent<RectTransform>();
//			canvas = view.GetComponent<CanvasGroup>();

//			moveOut = vertical ? slide1v : slide1h;
//			moveIn = vertical ? slide2v : slide2h;
//		}

//		public void Slide(bool reverse = false) => GoTo(index + (reverse ? -1 : 1));

//		public void GoToSilent(int index) {
//			if(data.Count == 0) return;
//			size = vertical ? rect.rect.height : rect.rect.width;
//			size *= this.index < index ? 1 : -1;
//			this.index = index.Repeat(data.Count);

//			if(view.isActiveAndEnabled && data.Count > 1)
//				view.OverrideRoutine(ref coroutine, ExtensionMethods.Progressor(moveOut, speed, ChangeData));
//			else view.Data = data[index];
//		}

//		public void GoTo(int index) {
//			GoToSilent(index);
//			OnIndexChanged?.Invoke(this.index);
//		}

//		void ChangeData() {
//			view.Data = data[index];
//			view.OverrideRoutine(ref coroutine, ExtensionMethods.Progressor(moveIn, speed));
//		}

//		void slide1h(float f) {
//			rect.localPosition = rect.localPosition.X(Mathf.Lerp(0, -size, f));
//			canvas.alpha = 1 - f;
//		}

//		void slide1v(float f) {
//			rect.localPosition = rect.localPosition.Y(Mathf.Lerp(0, size, f));
//			canvas.alpha = 1 - f;
//		}

//		void slide2h(float f) {
//			rect.localPosition = rect.localPosition.X(Mathf.Lerp(size, 0, f));
//			canvas.alpha = f;
//		}

//		void slide2v(float f) {
//			rect.localPosition = rect.localPosition.Y(Mathf.Lerp(-size, 0, f));
//			canvas.alpha = f;
//		}

//	}

//}