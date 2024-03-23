//using System;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.UIElements;

//namespace Drafts {

//	public class ElementView : VisualElement {

//		static StylePropertyName _propertyOpacity = "opacity";
//		static StylePropertyName _propertyDisplay = "display";
//		static StylePropertyName _propertyVisibility = "visibility";

//		object _data = null;
//		public object Data { get => _data; set { _data = value; Refresh(); } }
//		bool repaintOnTransition;

//		public ElementView() {
//			RegisterCallback<AttachToPanelEvent>(ev => OnAttach());
//			RegisterCallback<DetachFromPanelEvent>(ev => OnDetach());
//			RegisterCallback<TransitionStartEvent>(ev => TransitionStart());
//			RegisterCallback<TransitionStartEvent>(Transition);
//		}

//		private void Transition(TransitionStartEvent evt) {
//			foreach(var name in evt.stylePropertyNames)
//				Debug.Log(name.ToString());

//			if(evt.AffectsProperty(_propertyOpacity)
//			|| evt.AffectsProperty(_propertyDisplay)
//			|| evt.AffectsProperty(_propertyVisibility)) {
//				Debug.Log("tra start");
//			}
//		}

//		protected virtual void OnAttach() { }
//		protected virtual void OnDetach() { }
//		protected virtual void Repaint() { }
//		protected virtual void RepaintNull() { }

//		public virtual void Refresh() {
//			if(enabledInHierarchy) {
//				if(_data == null) RepaintNull();
//				else Repaint();
//			} else repaintOnTransition = true;
//		}

//		void TransitionStart() {
//			if(repaintOnTransition) Repaint();
//			repaintOnTransition = false;
//		}

//	}

//	public class ElementView<T> : ElementView {

//		//public new class UxmlFactory : UxmlFactory<CardView, UxmlTraits> { }

//		public new class UxmlTraits : VisualElement.UxmlTraits {
//			UxmlBoolAttributeDescription listenToSelection = new UxmlBoolAttributeDescription() { name = "listenToSelection", defaultValue = false };

//			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription { get { yield break; } }

//			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
//				base.Init(ve, bag, cc);
//				var ele = ve as ElementView<T>;
//				ele.listenToSelection = listenToSelection.GetValueFromBag(bag, cc);
//			}
//		}

//		static IReadOnlyList<T> _selection;
//		public static IReadOnlyList<T> Selection { get => _selection; set { _selection = value; OnSelectionChanged?.Invoke(value); } }
//		public static T Selected { get => Selection.FirstOrDefault(); set => Selection = new[] { value }; }
//		static event Action<IReadOnlyList<T>> OnSelectionChanged;

//		public new T Data { get => (T)base.Data; set => base.Data = value; }

//		protected bool listenToSelection { get; set; }

//		protected override void OnAttach() {
//			base.OnAttach();
//			if(listenToSelection) OnSelectionChanged += SetData;
//		}

//		public void SelectThis() => Selected = Data;

//		public void SetData(IEnumerable<T> values) => Data = values.FirstOrDefault();
//		public void SetData(T value) => Data = value;

//	}

//	public static class ExtensionsElementView {

//		public class MissingElementView<T> : Exception {
//			public MissingElementView(string element)
//				: base($"ElementView<{typeof(T).Name}> not found in {element}.") { }
//		}

//		public static ListView ListView<T>(this VisualElement template, List<T> list) {
//			if(template == null) return null;
//			var view = new ListView();
//			view.Bind(template.visualTreeAssetSource, list);
//			template.parent.Add(view);
//			template.parent.Remove(template);
//			return view;
//		}

//		public static void Bind<T>(this ListView view, VisualTreeAsset template, List<T> list) {

//			void BindItem(VisualElement e, int i) {
//				var v = e.Q<ElementView<T>>() ?? throw new MissingElementView<T>(template.name);
//				v.Data = list[i];
//			}

//			view.makeItem = template.Instantiate;
//			view.bindItem = BindItem;
//			view.itemsSource = list;
//		}

//	}
//}



