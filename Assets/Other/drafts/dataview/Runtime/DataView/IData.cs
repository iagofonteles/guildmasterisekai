using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Drafts {

	public interface IData {
		object Data { get; set; }
	}

	public interface IData<T> : IData {
		new T Data { get; set; }
		object IData.Data { get => Data; set => Data = value is T d ? d : default; }
	}

	public interface IDataView : IData {
		GameObject gameObject { get; }
		Transform transform { get; }
		//void Refresh();
	}

	public interface IDataView<T> : IDataView, IData<T> { }

	public static class ExtensionsIData {
		public static object GetData(this IData i) => i.Data;
		public static T GetData<T>(this IData<T> i) => i.Data;
		public static void SetData(this IData i, object data) => i.Data = data;
		public static void SetData<T>(this IData<T> i, T data) => i.Data = data;

		public static IData FirstWithData(this IEnumerable<IData> col, object data)
			=> col.FirstOrDefault(c => c?.Data == data);
		public static T FirstWithData<T>(this IEnumerable<T> col, object data) where T : Component
			=> col.FirstOrDefault(c => c.GetComponent<IData>()?.Data == data);
		public static GameObject FirstWithData(this IEnumerable<GameObject> col, object data)
			=> col.FirstOrDefault(c => c.GetComponent<IData>()?.Data == data);
	}
}
