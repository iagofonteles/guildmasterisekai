using UnityEngine;
namespace Drafts.Patterns {

	/// <summary>
	/// A Monobehaviour Singleton Solution
	/// Set an instance on Awake. Needs to be on the scene at start.
	/// Is destroyed on scene change but can use static fields to persist data.
	/// Use HasInstance at awake to avoid calculations twice.
	/// </summary>
	public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T> {
		public static T Instance { get; private set; }
		public T Instanced => Instance ? Instance : Instance = (T)Instantiate(this);
		protected virtual void Awake() => Instance = (T)this;

		/// <summary>When called, destroy the current gameobject if an instance already exists or set the instance to this.</summary>
		protected bool HasInstance {
			get {
				if(Instance) {
					Destroy(gameObject);
					return true;
				} else {
					Instance = (T)this;
					return false;
				}
			}
		}
	}
}
