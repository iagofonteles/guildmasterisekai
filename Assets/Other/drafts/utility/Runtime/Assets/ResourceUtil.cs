using UnityEngine;
namespace Drafts {
	public static partial class ResourceUtil {

		public class ResourceNotFoundException<T> : System.Exception {
			public ResourceNotFoundException(string path) : base($"({typeof(T).Name}) + {path}") { }
		}

		public static T Instance<T>(ref T instance, string path = null, bool dontDestroyOnLoad = false) where T : Component {
			if(instance) return instance;
			var res = LoadPrefab<T>(path ?? typeof(T).Name);
			instance = Object.Instantiate(res.GetComponent<T>());
			instance.gameObject.SetActive(true);
			if(dontDestroyOnLoad) Object.DontDestroyOnLoad(instance);
			return instance;
		}

		public static T Instantiate<T>(string path = null, Transform parent = null, Vector3 position = default) where T : Component {
			var res = LoadPrefab<T>(path ?? typeof(T).Name);
			var instance = Object.Instantiate(res.GetComponent<T>());
			instance.gameObject.SetActive(true);
			instance.transform.parent = parent;
			instance.transform.localPosition = position;
			return instance;
		}

		public static T Load<T>(string path) where T : Object
			=> Resources.Load<T>(path) ?? throw new ResourceNotFoundException<T>(path);

		public static T Load<T>() where T : Object
			=> Resources.Load<T>(typeof(T).Name) ?? throw new ResourceNotFoundException<T>(typeof(T).Name);

		public static T LoadPrefab<T>(string path) where T : Component {
			var res = Resources.Load<GameObject>(path ?? typeof(T).Name);
			return res?.GetComponent<T>() ?? throw new ResourceNotFoundException<T>(path);
		}
	}
}
