using Drafts.Menus;
using UnityEngine;
using UnityEngine.Events;

namespace Drafts.Templates {
	public class PageSettings : MonoBehaviour {
		public MenuSettings menu;
		public string title, subtitle;
		public UnityEvent back, prev, next;
		Page page;

		private void Awake() {
			page = GetComponent<Page>();
			if(page) page.callbacks.onOpening.AddListener(SetThis);
			if(!menu) menu = transform.parent.GetComponent<MenuSettings>();
		}
		private void OnDestroy() { if(page) page.callbacks.onOpening.RemoveListener(SetThis); }
		public void SetThis() => menu.Set(this);
		public void SetThisIf(bool setIf) { if(setIf) menu.Set(this); }
	}

}
