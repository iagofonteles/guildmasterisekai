#if DRAFTS_UTILITY
using Drafts;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class ExtensionsUI {

	public static void Enable(this Selectable selectable) => SetEnabled(selectable, true);
	public static void Disable(this Selectable selectable) => SetEnabled(selectable, false);
	public static void SetEnabled(this Selectable selectable, bool value) {
		selectable.enabled = value;
		if(selectable.targetGraphic) selectable.targetGraphic.enabled = value;
	}

	public static void SelectDelayed(this Selectable selectable) => _SelectDelayed(selectable.gameObject).Start();
	public static void SelectDelayed(this GameObject selectable) => _SelectDelayed(selectable).Start();
	static IEnumerator _SelectDelayed(GameObject selectable) {
		EventSystem.current?.SetSelectedGameObject(null);
		yield return null;
		yield return null;
		if(selectable) EventSystem.current?.SetSelectedGameObject(selectable);
	}
}
#endif