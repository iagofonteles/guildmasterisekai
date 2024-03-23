#if ENABLE_INPUT_SYSTEM
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Drafts.UI.Flow.UIAction;
namespace Drafts.UI.Flow {

	public class UIActions : Selectable {
		public static string DefaultLClick;
		public static string DefaultRClick;
		public static string DefaultMClick;

		[SerializeField] SelectablePreset preset;
		[SerializeField] bool registerGlobally;
		[SerializeField] List<UIAction> actions = new();
		public IReadOnlyList<UIAction> Actions => actions;
		public UnityEvent<bool> OnInteractableChanged;

		protected override void DoStateTransition(SelectionState state, bool instant) {
			base.DoStateTransition(state, instant);
			OnInteractableChanged?.Invoke(interactable);
		}

		protected override void Awake() {
			base.Awake();
			SetPreset(preset);
		}

		protected override void OnEnable() {
			base.OnEnable();
			if(registerGlobally) Subscribe();
		}

		protected override void OnDisable() {
			base.OnDisable();
			if(registerGlobally) Unsubscribe();
		}

		public void SetPreset(SelectablePreset preset) {
			if(!preset) return;
			spriteState = preset.sprites;
			animationTriggers = preset.triggers;
			colors = preset.colors;
			transition = preset.transition;
		}

		protected override void Start() {
			base.Start();
			if(!Application.isPlaying || !preset) return;
			targetGraphic = preset.Clone(transform);
		}

		protected override void OnDestroy() { base.OnDestroy(); if(Current == actions) Current = null; }
		public void Subscribe() => Add(actions);
		public void Unsubscribe() => Remove(actions);
		public void SetActive(bool active) { if(active) Subscribe(); else Unsubscribe(); }
		public void AddAction(string control, UnityAction action) {
			var a = actions.First(a => a.Control == control);
			a.Pressed.AddListener(action);
		}
		public void AddAction(UIAction action) => actions.Add(action);

		public override void OnSelect(BaseEventData eventData) { base.OnSelect(eventData); Current = actions; }
		public override void OnDeselect(BaseEventData eventData) { base.OnDeselect(eventData); if(Current == actions) Current = null; }
		public override void OnPointerEnter(PointerEventData eventData) { base.OnPointerEnter(eventData); Select(); }
		public override void OnPointerDown(PointerEventData eventData) {
			//base.OnPointerDown(eventData);
			if(!IsInteractable()) return;
			if(eventData.button == PointerEventData.InputButton.Right) Execute(DefaultRClick, false);
			if(eventData.button == PointerEventData.InputButton.Left) Execute(DefaultLClick, false);
			if(eventData.button == PointerEventData.InputButton.Middle) Execute(DefaultMClick, false);
		}
		public override void OnPointerUp(PointerEventData eventData) {
			//base.OnPointerDown(eventData);
			if(!IsInteractable()) return;
			if(eventData.button == PointerEventData.InputButton.Right) Execute(DefaultRClick, true);
			if(eventData.button == PointerEventData.InputButton.Left) Execute(DefaultLClick, true);
			if(eventData.button == PointerEventData.InputButton.Middle) Execute(DefaultMClick, true);
		}

#if UNITY_EDITOR
		protected override void Reset() { base.Reset(); transition = Transition.None; }
#endif

	}
}
#endif