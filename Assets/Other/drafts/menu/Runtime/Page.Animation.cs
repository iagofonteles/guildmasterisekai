using Drafts.Patterns;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Drafts.Menus {

	[DisallowMultipleComponent]
	public partial class Page {

		#region Fields
		enum BehaviourOnStart { None, Hide, Open }
		enum Deactivate { None = 0, GameObject = 1, Canvas = 2, Destroy = 4 }
		enum AnimationType { Tweak, Animator }

		public Page parent;
		[SerializeField] BehaviourOnStart behaviourOnStart = BehaviourOnStart.Hide;
		[SerializeField] Deactivate deactivate = Deactivate.GameObject;

		[SerializeField] UnityEngine.Object openAnimationObj;
		[SerializeField] UnityEngine.Object closeAnimationObj;
		IAnimationProvider openAnimation;
		IAnimationProvider closeAnimation;
		[SerializeField] bool waitAnimation;
		[SerializeField] internal Conditional<Vector2> customStartPosition = new Conditional<Vector2>() { active = true };

		[Serializable]
		public class Callbacks {
			public UnityEvent<bool> onChanging;
			public UnityEvent onOpening;
			public UnityEvent onOpened;
			public UnityEvent onClosing;
			public UnityEvent onClosed;
		}
		public Callbacks callbacks;

		#endregion

		void Awake() {
			openAnimation = openAnimationObj as IAnimationProvider;
			closeAnimation = closeAnimationObj as IAnimationProvider;
			if(parent) parent.children.Add(this);
			if(customStartPosition) transform.localPosition = customStartPosition.value;
		}

		protected virtual void Start() {
			if(behaviourOnStart == BehaviourOnStart.Hide) {
				IsOpen = false;
				closeAnimation.Play(true);
				CallCallbacks(false, false);
				return;
			}
			if(behaviourOnStart == BehaviourOnStart.Open) {
				closeAnimation.Play(true);
				IsOpen = false;
				Open();
				return;
			}
			IsOpen = gameObject.activeSelf;
		}

		void CallCallbacks(bool open, bool start) {
			if(start) {
				if(open) {
					gameObject.SetActive(true);
					callbacks.onOpening.Invoke();
					if(deactivate.HasFlag(Deactivate.Canvas)) GetComponent<Canvas>().enabled = true;
				} else callbacks.onClosing.Invoke();
			} else {
				if(open) callbacks.onOpened.Invoke();
				else {
					callbacks.onClosed.Invoke();
					if(deactivate.HasFlag(Deactivate.GameObject)) gameObject.SetActive(false);
					if(deactivate.HasFlag(Deactivate.Canvas)) gameObject.GetComponent<Canvas>().enabled = false;
					if(deactivate.HasFlag(Deactivate.Destroy)) Destroy(gameObject);
				}
			}
		}

		void SetStartPosition() => customStartPosition = new Conditional<Vector2>() { active = true, value = transform.position };
	}
}
