#if ENABLE_INPUT_SYSTEM
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Drafts.UI.Flow {
	[Serializable]
	public class UIAction {
		public static bool LockAll;

		[SerializeField] string control;
		[SerializeField] string name;
		[SerializeField] UnityEvent action;
		[SerializeField] DraftsFunc<bool> validation;
		[SerializeField] UnityEvent onReleased;

		public string Name => name;
		public string Control => control;
		public UnityEvent Pressed => action;
		public UnityEvent Released => onReleased;
		public bool IsValid => !validation.IsValid || validation.Invoke();

		public UIAction() { }
		public UIAction(string control, string name, UnityAction action, UnityAction cancelled, DraftsFunc<bool> validation = null) {
			this.control = control;
			this.name = name;
			this.action = new();
			onReleased = new();
			this.validation = validation ?? new();
			if(action != null) this.action.AddListener(action);
			if(cancelled != null) onReleased.AddListener(cancelled);
		}

		public static InputActionMap ActionMap { get; private set; }
		static IEnumerable<UIAction> Empty = new UIAction[0];
		static IEnumerable<UIAction> current = Empty;
		static HashSet<IEnumerable<UIAction>> queued = new();
		public static Action OnChanged;
		public static IEnumerable<UIAction> Current { get => current; set { current = value ?? Empty; OnChanged?.Invoke(); } }
		public static IEnumerable<UIAction> Queued => queued.SelectMany(i => i);
		public static IEnumerable<UIAction> All => current.Concat(queued.SelectMany(i => i));
		public static void Add(IEnumerable<UIAction> actions) { queued.Add(actions); OnChanged?.Invoke(); }
		public static void Remove(IEnumerable<UIAction> actions) { queued.Remove(actions); OnChanged?.Invoke(); }

		public static void SetActionMap(InputActionMap m) {
			if(ActionMap == m) return;
			if(ActionMap != null) foreach(var a in ActionMap) a.performed -= Execute;
			ActionMap = m;
			if(ActionMap != null) foreach(var a in ActionMap) a.performed += Execute;
		}

		static void Execute(InputAction.CallbackContext ctx) => Execute(ctx.action.name, ctx.canceled);

		public static void Execute(string control, bool cancelled) {
			if(LockAll) return;
			var action = All.FirstOrDefault(a => a.Control == control);
			if(action == null || !action.IsValid) return;
			if(cancelled) action.Released.Invoke();
			else action.Pressed.Invoke();
			Current = current;
		}
	}
}
#endif