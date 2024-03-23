using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Drafts.UI {

	[RequireComponent(typeof(CanvasGroup))]
	public class SelectableRoot : MonoBehaviour {
		static List<SelectableRoot> All { get; } = new();

		[SerializeField] Selectable first;
		GameObject last;
		CanvasGroup group;

		private void Awake() {
			All.RemoveAll(s => !s);
			group = GetComponent<CanvasGroup>();
		}

		private void OnEnable() {
			if(All.Count > 0) All[0].SaveSelection();
			LoadSelection();
			All.Insert(0, this);
		}

		private void OnDisable() {
			if(All.Count > 0 && All[0] == this) {
				SaveSelection();
				if(All.Count > 1) All[1].LoadSelection();
			}
			All.Remove(this);
		}

		void LoadSelection() {
			group.interactable = true;
			var go = last ? last : (first ? first.gameObject : null);
			if(go) EventSystem.current.SetSelectedGameObject(go);
		}

		void SaveSelection() {
			group.interactable = false;
			last = EventSystem.current.currentSelectedGameObject;
		}
	}
}
