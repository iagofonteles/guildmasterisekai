using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UObj = UnityEngine.Object;

namespace Drafts {

	public static class ExtensionsDataView {

		public static List<T> Clone<T>(this T template, int count, Action<T> setup = null, Transform parent = null) where T : Component {
			var result = new List<T>();
			if(!template) return result;

			template.gameObject.SetActive(true);
			if(parent == null) template.DestroySiblings();
			parent ??= template.transform.parent;

			for(int i = 0; i < count; i++) {
				var t = UObj.Instantiate(template, parent);
				setup?.Invoke(t);
				result.Add(t);
			}
			template.gameObject.SetActive(false);
			return result;
		}

		public static List<T> Template<T, D>(this T template, IEnumerable<D> data, Action<T, D> setup, Transform parent = null) where T : Component {
			var result = new List<T>();
			if(!template) return result;

			template.gameObject.SetActive(true);
			if(parent == null) template.DestroySiblings();
			parent ??= template.transform.parent;

			foreach(var d in data) {
				var t = UObj.Instantiate(template, parent);
				setup(t, d);
				result.Add(t);
			}
			template.gameObject.SetActive(false);
			return result;
		}

		public static T Template<T, D>(this T template, D data, Action<T, D> setup) where T : Component {
			if(!template) return null;
			template.gameObject.SetActive(true);

			var t = UObj.Instantiate(template, template.transform.parent);
			setup(t, data);

			template.gameObject.SetActive(false);
			return t;
		}

		/// <summary>To be used with null mode destroy.</summary>
		public static List<V> Template<V, T>(this V view, IEnumerable<T> data, Transform parent = null) where V : Component, IData<T> {
			var ret = new List<V>();
			if(!view) return ret;

			view.gameObject.SetActive(true);
			if(parent == null) view.DestroySiblings();

			var index = 0;
			foreach(var item in data) {
				var obj = view.Template(item, parent);
				if(obj is IIndex i) i.Index = index++;
				ret.Add(obj);
			}

			view.gameObject.SetActive(false);
			return ret;
		}

		/// <summary>To be used with null mode destroy.</summary>
		public static List<V> Template<V>(this V view, IEnumerable data, Transform parent = null) where V : Component, IData {
			var ret = new List<V>();
			if(!view) return ret;

			view.gameObject.SetActive(true);
			if(parent == null) view.DestroySiblings();

			var index = 0;
			foreach(var item in data) {
				var obj = view.Template(item, parent);
				if(obj is IIndex i) i.Index = index++;
				ret.Add(obj);
			}

			view.gameObject.SetActive(false);
			return ret;
		}

		/// <summary>To be used with null mode destroy.</summary>
		public static V Template<V, T>(this V view, T data, Transform parent = null) where V : Component, IData<T> {
			var ret = UObj.Instantiate(view, parent ?? view.transform.parent);
			ret.gameObject.SetActive(true);
			ret.Data = data;
			return ret;
		}

		/// <summary>To be used with null mode destroy.</summary>
		public static V Template<V>(this V view, object data, Transform parent = null) where V : Component, IData {
			var ret = UObj.Instantiate(view, parent ?? view.transform.parent);
			ret.gameObject.SetActive(true);
			ret.Data = data;
			return ret;
		}

		/// <summary>To be used with null mode destroy.</summary>
		public static V Template<V, T>(this V view, T data, Vector3 position, Quaternion? rotation = null, Transform parent = null) where V : Component, IData<T> {
			var ret = UObj.Instantiate(view, position, rotation ?? Quaternion.identity, parent ?? view.transform.parent);
			ret.gameObject.SetActive(true);
			ret.Data = data;
			return ret;
		}

		public static void SetData<T>(this IEnumerable<IData<T>> views, IEnumerable<T> values) {
			var l = views.GetEnumerator();
			var v = values.GetEnumerator();
			while(l.MoveNext())
				l.Current.Data = v.MoveNext() ? v.Current : default;
		}

		public static void SetActive<T, D>(this IEnumerable<T> views, Predicate<D> predicate) where T : DataView<D, T> {
			foreach(var item in views) item.gameObject.SetActive(predicate(item.Data));
		}

		public static void TrySetData<T>(this DataView<T> view, T data) { if(view) view.Data = data; }
		public static void TrySetData(this DataHolder view, object data) { if(view) view.Data = data; }
		public static void TryRefresh(this DataView view) { if(view) view.Refresh(); }

		public static List<V> TryTemplate<V>(this V view, IEnumerable data, Transform parent = null) where V : Component, IData => view ? Template(view, data, parent) : new();
		public static List<V> TryTemplate<V, T>(this V view, IEnumerable<T> data, Transform parent = null) where V : Component, IData<T> => view ? Template(view, data, parent) : new();

		public static void DestroySiblings(this Component c) {
			for(int i = 1; i < c.transform.parent.childCount; i++)
				UObj.Destroy(c.transform.parent.GetChild(i).gameObject);
		}
	}

}
