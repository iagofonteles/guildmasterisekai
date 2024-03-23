using System;
using UnityEngine;
namespace Drafts.DAnima {

	class MonoCallback : MonoBehaviour {
		public event Action OnEnabled;
		public event Action OnDisabled;
		public event Action OnChildrenChanged;
		private void OnEnable() => OnEnabled?.Invoke();
		private void OnDisable() => OnDisabled?.Invoke();
		private void OnTransformChildrenChanged() => OnChildrenChanged?.Invoke();
	}

	public class Behaviour : Anima.IModule {
		public GameObject target;
		public bool deativateOnClose;
		public bool playOnActivate;

		public void Subscribe(Anima anima) {
			if(!target) target = anima.gameObject;

			if(deativateOnClose) {
				anima.callbacks.OnPlay.AddListener(Activate);
				anima.callbacks.OnFinish.AddListener(target.SetActive);
			}

			if(playOnActivate) {
				var mono = target.GetComponent<MonoCallback>() ?? target.AddComponent<MonoCallback>();
				mono.OnEnabled += anima.Play;
				anima.Play();
			}

			anima.modules.Remove(this);
		}

		void Activate(bool _) => target.SetActive(true);
	}

	public class MenuWindow : Anima.IModule, ISingleLineInspector {
		float ISingleLineInspector.RightPadding => 60;
		public CanvasGroup target;

		public void Subscribe(Anima anima) {
			if(!target) target = anima.GetComponent<CanvasGroup>();
			anima.callbacks.OnPlay.AddListener(Start);
			anima.callbacks.OnFinish.AddListener(Finish);
			anima.enabled = false;
			anima.SetProgress(0);
		}

		void Start(bool active) {
			target.blocksRaycasts = false;
			target.gameObject.SetActive(true);
		}

		void Finish(bool active) {
			target.blocksRaycasts = active;
			target.gameObject.SetActive(active);
		}
	}

	public class StartClosed : Anima.IModule {
		public void Subscribe(Anima anima) {
			anima.enabled = false;
			anima.SetProgress(0);
		}
	}

	public class StartOpened : Anima.IModule {
		public void Subscribe(Anima anima) {
			anima.SetProgress(1);
		}
	}
}
