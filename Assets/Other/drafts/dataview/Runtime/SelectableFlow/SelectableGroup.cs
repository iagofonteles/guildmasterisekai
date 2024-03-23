using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Drafts.UI {

	public class SelectableGroup : MonoBehaviour {
		static List<SelectableGroup> stack = new();

		public List<Selectable> itens;
		public UnityEvent<bool> OnCurrentChanged;
		public UnityEvent<bool> OnPushedToStack;

		public bool deativateItensOnAwake = true;
		public bool pushOnStart = true;

		[NonSerialized] GameObject last;

		public bool InStack => stack.Contains(this);
		private void Awake() { if(deativateItensOnAwake) foreach(var s in itens) s.Disable(); }
		private void Start() { if(pushOnStart) Push(); }
		private void OnDestroy() => stack.Remove(this);
		private void Reset() => enabled = false;
		public void SelectFirst() {
			var item = itens.FirstOrDefault();
			if(item) item.SelectDelayed();
			else PopIfCurrent();
		}

		GameObject GetItem() => last && last.activeInHierarchy ? last : itens.FirstOrDefault(s => s.gameObject.activeInHierarchy)?.gameObject;

		public void OnEnable() {
			foreach(var s in itens) s.Enable();
			OnCurrentChanged.Invoke(true);
			GetItem().SelectDelayed();
		}

		public void OnDisable() {
			last = EventSystem.current?.currentSelectedGameObject;
			foreach(var s in itens) s.Disable();
			OnCurrentChanged.Invoke(false);
		}

		public void Push() {
			if(!GetItem()) return;
			stack.RemoveAll(s => !s);

			if(stack.Count > 0) {
				if(stack[0] == this) return;
				stack[0].enabled = false;
				OnPushedToStack.Invoke(false);
			}
			stack.Insert(0, this);
			enabled = true;
			OnPushedToStack.Invoke(true);
		}

		public static void Pop() {
			if(stack.Count < 1) return;
			stack[0].enabled = false;
			stack.RemoveAt(0);
			if(stack.Count < 1) return;
			stack[0].enabled = true;
		}

		public void PopIfCurrent() { if(enabled) Pop(); }

		public void PopToThis() {
			if(!stack.Contains(this)) return;
			while(stack.Count > 1 && stack[0] != this) {
				stack[0].enabled = false;
				stack.RemoveAt(0);
			}
			if(stack.Count > 1) stack[0].enabled = true;
		}
	}
}
