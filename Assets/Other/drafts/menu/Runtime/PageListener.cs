using Drafts.Menus;
using System;
using UnityEngine;
using UnityEngine.Events;

public class PageListener : MonoBehaviour {

	enum PageState { Close, Open, Toggle }

	[Serializable]
	class Callback {
		public string name;
		public PageState state;
		public UnityEvent<bool> callback;
	}

	[SerializeField] Callback[] callbacks;

	private void Awake() => Page.OnChanged += OnPageChanged;

	void OnPageChanged(string name, bool active) {

		foreach(var c in callbacks) {
			if(c.name != name) continue;
			if(c.state == PageState.Toggle) c.callback.Invoke(active);
			if((active ? 1 : 0) == (int)c.state) c.callback.Invoke(active);
		}
	}
}
