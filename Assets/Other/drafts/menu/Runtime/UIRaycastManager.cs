using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Drafts.Tweens {
	public static class Documents {

		[Obsolete]
		public static string RandomCPF() {
			int i;
			var ret = new List<int>();
			for(i = 0; i < 9; i++) ret.Add(UnityEngine.Random.Range(0, 10));

			i = 0;
			var resto = ret.Sum(v => v * 10 - i++) % 11;
			ret.Add(resto < 2 ? 0 : 11 - resto);

			i = 0;
			resto = ret.Sum(v => v * 11 - i++) % 11;
			ret.Add(resto < 2 ? 0 : 11 - resto);

			return string.Join("", ret.Cast<string>());
		}

	}

	/// <summary>Disable root transform CanvasGroup raycast on Begin and enable on End.</summary>
	[Obsolete]
	public static class UIRaycastManager {
		static Dictionary<CanvasGroup, HashSet<Component>> activeTweens = new Dictionary<CanvasGroup, HashSet<Component>>();

		public static void Disable(Component c) {
			var root = c.transform.root.GetComponent<CanvasGroup>();
			if(!activeTweens.ContainsKey(root)) activeTweens[root] = new HashSet<Component>();
			activeTweens[root].Add(c);
			root.blocksRaycasts = false;
			//Debug.Log($"{activeTweens[root].Count} disable: {c.name}");
		}

		public static void Enable(Component c) {
			try {
				var root = c.transform.root.GetComponent<CanvasGroup>();
				activeTweens[root].Remove(c);
				root.blocksRaycasts = activeTweens[root].Count == 0;
				//Debug.Log($"{activeTweens[root].Count} enable: {c.name}");
			} catch { }
		}
	}

}
