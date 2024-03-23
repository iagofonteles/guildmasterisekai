using UnityEngine;
namespace Drafts {

	public partial class Anima : MonoBehaviour {
		public interface IAnim {
			void Subscribe(Anima anima);
			void Update(float time);
		}
		public interface IModule : IAnim {
			void IAnim.Update(float time) { }
		}
	}

	namespace DAnima {
		public abstract class Base<T> : Anima.IAnim where T : Component {
			public T target;
			public virtual void Subscribe(Anima anima) {
				if(!target) target = anima.TryGetComponent<T>(out var v) ? v : anima.gameObject.AddComponent<T>();
			}
			public abstract void Update(float time);
		}
		public abstract class Base<T, V> : Base<T> where T : Component {
			public V start, end;
		}
	}
}
