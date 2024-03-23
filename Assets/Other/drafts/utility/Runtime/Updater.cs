using UnityEngine;
using System;
using System.Collections;

namespace Drafts {

	/// <summary>A single session persistent MonoBehaviour that send Unity's events to all subscribers.</summary>
	public partial class Updater : MonoBehaviour {
		static Updater _instance;
		public static Updater Instance => _instance ? _instance : _instance = GetInstance();

		static Updater GetInstance() {
			_instance = new GameObject("Singleton Updater").AddComponent<Updater>();
			DontDestroyOnLoad(_instance.gameObject);
			return _instance;
		}

		//public static Coroutine Coroutine(IEnumerator routine) => Instance.StartCoroutine(routine);
		public static Coroutine Invoke(Action action) => Instance.StartCoroutine(_Invoke(action));
		public static Coroutine Invoke(float time, Action action) => Instance.StartCoroutine(_Invoke(time, action));
		public static Coroutine Invoke(Func<bool> waitUntil, Action action) => Instance.StartCoroutine(_Invoke(waitUntil, action));

		static IEnumerator _Invoke(Func<bool> waitUntil, Action action) {
			yield return new WaitUntil(waitUntil);
			action();
		}

		static IEnumerator _Invoke(Action action) {
			yield return null;
			action();
		}

		static IEnumerator _Invoke(float time, Action action) {
			yield return new WaitForSeconds(time);
			action();
		}

		#region Events
		public static event Action StartEvent = () => { };
		public static event Action EnableEvent = () => { };
		public static event Action DisableEvent = () => { };
		public static event Action DestroyEvent = () => { };

		public static event Action FixedUpdateEvent = () => { };
		public static event Action UpdateEvent = () => { };
		public static event Action LateUpdateEvent = () => { };

		public static event Action<bool> ApplicationFocusEvent = b => { };
		public static event Action<bool> ApplicationPauseEvent = b => { };
		public static event Action ApplicationQuitEvent = () => { };

		public static event Action DrawGizmosEvent = () => { };
		public static event Action GUIEvent = () => { };
		public static event Action PostRenderEvent = () => { };
		public static event Action PreCullEvent = () => { };
		public static event Action PreRenderEvent = () => { };

#pragma warning disable IDE0051 // Remove unused private members
		private void Start() => StartEvent();
		private void OnEnable() => EnableEvent();
		private void OnDisable() => DisableEvent();
		private void OnDestroy() => DestroyEvent();

		private void FixedUpdate() => FixedUpdateEvent();
		private void Update() => UpdateEvent();
		private void LateUpdate() => LateUpdateEvent();

		private void OnApplicationFocus(bool hasFocus) => ApplicationFocusEvent(hasFocus);
		private void OnApplicationPause(bool pauseStatus) => ApplicationPauseEvent(pauseStatus);
		private void OnApplicationQuit() => ApplicationQuitEvent();

		private void OnDrawGizmos() => DrawGizmosEvent();
		private void OnGUI() => GUIEvent();
		private void OnPostRender() => PostRenderEvent();
		private void OnPreCull() => PreCullEvent();
		private void OnPreRender() => PreRenderEvent();
#pragma warning restore IDE0051 // Remove unused private members
		#endregion

	}

}