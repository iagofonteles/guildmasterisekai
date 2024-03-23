using UnityEngine;

namespace Drafts.Editing.Brushes {
	public class Pixel : Brush {
		public Pixel(IManipulator manipulator) : base(manipulator) { }
		public override string Name => "Pixel";

		public override bool Click(bool alt) {
			var pos = alt ? InPoint : OutPoint;
			if(Once(alt, pos)) return false;
			OnClick(alt, pos);
			return true;
		}

		public override bool Drag(bool alt, Vector3 delta) {
			var pos = alt ? InPoint : OutPoint;
			if(Once(alt, pos) || pos.y != lastPosition.y) return false;
			OnClick(alt, pos);
			return true;
		}

		protected virtual void OnClick(bool alt, Vector3 point) {
			Manager.SetTile(point, alt ? null : Tile);
			Manager.Update();
		}
	}
}
