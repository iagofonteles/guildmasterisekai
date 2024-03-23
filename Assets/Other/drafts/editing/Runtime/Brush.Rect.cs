using UnityEngine;

namespace Drafts.Editing.Brushes {
	public class Box : Brush {
		public Box(IManipulator manipulator) : base(manipulator) { }
		public override string Name => "Box";

		Vector3? first;

		public override bool Click(bool alt)
			=> Click(alt, alt ? InPoint : OutPoint);

		bool Click(bool alt, Vector3 point) {
			if(first == null) {
				first = point;
				return true;
			}
			var pos = point;
			var delta = first.Value - pos;
			Fill(pos, delta, alt);
			first = null;
			return true;
		}

		public override void Update(bool alt) {
			if(first == null) {
				Gizmo.Size = Vector2.zero;
				return;
			}
			var delta = OutPoint - first.Value;
			var size = new Vector3(
				Mathf.Abs(delta.x) + Manager.TileSize.x + 0.1f,
				Mathf.Abs(delta.y) + Manager.TileSize.y + 0.1f,
				Mathf.Abs(delta.z) + Manager.TileSize.z + 0.1f);
			Gizmo.Size = size;
			Gizmo.Position = first.Value + delta / 2;
		}

		public override void OnDisable() {
			Cancel();
		}

		public override void Cancel() {
			first = null;
			Gizmo.Size = Vector3.zero;
		}

		public void Fill(Vector3 pos, Vector3 delta, bool alt) {
			var min = new Vector3(
				delta.x < 0 ? pos.x + delta.x : pos.x,
				delta.y < 0 ? pos.y + delta.y : pos.y,
				delta.z < 0 ? pos.z + delta.z : pos.z
			);
			var max = new Vector3(
				delta.x > 0 ? pos.x + delta.x : pos.x,
				delta.y > 0 ? pos.y + delta.y : pos.y,
				delta.z > 0 ? pos.z + delta.z : pos.z
			);

			for(var i = min.x; i <= max.x; i++)
				for(var j = min.y; j <= max.y; j++)
					for(var k = min.z; k <= max.z; k++)
						FillStep(alt, new(i, j, k));

			FillEnd(alt, min, max);
		}

		protected virtual void FillStep(bool alt, Vector3 pos) => Manager.SetTile(pos, alt ? null : Tile);
		protected virtual void FillEnd(bool alt, Vector3 min, Vector3 max) => Manager.Update();
	}
}
