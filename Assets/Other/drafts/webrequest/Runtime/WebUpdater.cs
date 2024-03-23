using System.Collections;
using UnityEngine;

public static class StartCoroutine {
	class WebUpdater : MonoBehaviour { }
	static WebUpdater updater;
	static WebUpdater Updater => updater ??= CreateUpdaterC();
	static WebUpdater CreateUpdaterC() {
		var go = new GameObject("Web Coroutine");
		Object.DontDestroyOnLoad(go);
		return go.AddComponent<WebUpdater>();
	}
	public static Coroutine Start(this IEnumerator ie) => Updater.StartCoroutine(ie);
}
