using UnityEngine;
namespace Drafts.Editing {
	public abstract class ChunkManagerView : MonoBehaviour {
		public abstract IChunkManager ChunkManager { get; }
		public virtual void Subscribe() { }
		public virtual void Unsubscribe() { }
	}
}
