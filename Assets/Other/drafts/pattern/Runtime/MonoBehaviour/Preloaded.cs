using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Drafts.Patterns {

	/// <summary>Use it the show your script on PrefabPool.</summary>
	public interface IPreloaded { }

	/// <summary>Used by to .</summary>
	public class Preloaded : ScriptableObject {

		static Preloaded instance;
		static Preloaded Instance => instance ??= GetInstance();
		[SerializeField] UnityEngine.Object[] assets;

		static Preloaded GetInstance() {
#if UNITY_EDITOR
			var ret = PlayerSettings.GetPreloadedAssets().FirstOrDefault(a => a is Preloaded) as Preloaded;
#else
			var ret = Resources.FindObjectsOfTypeAll<Preloaded>().FirstOrDefault();
#endif
			if(!ret) throw new Exception("Preloaded instance not found in preloaded assets");
			return ret;
		}

		/// <summary>Return a new duplicate of the found prefab.</summary>
		public static T Instantiate<T>(Transform parent = null) where T : Component, IPreloaded => Instantiate(GetPefab<T>(), parent);

		/// <summary>Store result if you pretend to call multiple times.</summary>
		public static T GetPefab<T>() where T : Component, IPreloaded {
			var ret = Instance.assets.FirstOrDefault(a => a is GameObject go && go.GetComponent<T>());
			if(!ret) throw new Exception($"No prefab of type {typeof(T).Name} found in Preloaded instance.");
			return (ret as GameObject).GetComponent<T>();
		}

		/// <summary>Store result if you pretend to call multiple times.</summary>
		public static T GetAsset<T>() where T : UnityEngine.Object, IPreloaded {
			var ret = Instance.assets.FirstOrDefault(a => a is T);
			if(!ret) throw new Exception($"No asset of type {typeof(T).Name} found in Preloaded instance.");
			return ret as T;
		}
	}

}
