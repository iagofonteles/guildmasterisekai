using UnityEngine;

namespace Drafts.Editing {
	public abstract class Brush {
		public Brush(IManipulator manipulator) => Manipulator = manipulator;
		public abstract string Name { get; }
		protected IManipulator Manipulator { get; }

		protected ITile Tile => Manipulator.Tile;
		protected IGizmo Gizmo => Manipulator.Gizmo;
		protected IChunkManager Manager => Manipulator.ChunkManager;
		protected Vector3 OutPoint => Manipulator.FreePosition;
		protected Vector3 InPoint => Manipulator.Position;

		protected Vector3 lastPosition;
		protected ITile lastTile;
		protected bool lastAlt;

		public virtual void OnEnable() { }
		public virtual void OnDisable() { }
		public virtual void Cancel() { }
		public virtual void Update(bool alt) { }
		public virtual bool Click(bool alt) => false;
		public virtual bool Hold(bool alt) => false;
		public virtual bool Drag(bool alt, Vector3 delta) => false;
		public virtual bool Release(bool alt) => true;

		protected virtual bool Once(bool alt, Vector3 pos) {
			if(pos == lastPosition && Tile == lastTile && alt == lastAlt) return true;
			lastPosition = pos;
			lastTile = Tile;
			lastAlt = alt;
			return false;
		}
	}
}
