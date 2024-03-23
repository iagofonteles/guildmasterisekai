//using System;
//using UnityEngine;

//public class DataSelection<T> : MonoBehaviour {

//	static T _selected;
//	public static T Selected { get => _selected; set { _selected = value; OnSelectedChanged?.Invoke(value); } }
//	static event Action<T> OnSelectedChanged;

//	[SerializeField] bool listenToSelected;

//	/// <summary>Subscribe, Refresh & Listen to Selected.</summary>
//	protected virtual void Start() {
//		if(!listenToSelected) return;
//		OnSelectedChanged += _SetDataOnSelected;
//		Data = Selected;
//	}

//	protected override void OnDestroy() {
//		base.OnDestroy();
//		if(listenToSelected) OnSelectedChanged -= _SetDataOnSelected;
//	}

//	void _SetDataOnSelected(T data) => Data = data;
//	public void SelectThis() => Selected = Data;
//}
