using UnityEngine;
namespace Drafts.DAnima {

	public class Anchors : Base<RectTransform> {
		public Vector2 startMin, startMax = Vector2.one;
		public Vector2 endMin, endMax = Vector2.one;

		public override void Update(float time) {
			if(!target) return;
			target.anchorMin = Vector2.LerpUnclamped(startMin, endMin, time);
			target.anchorMax = Vector2.LerpUnclamped(startMax, endMax, time);
		}
	}

	public class LocalPosition : Base<RectTransform, Vector2> {
		public override void Update(float time) {
			if(!target) return;
			target.localPosition = Vector2.LerpUnclamped(start, end, time);
		}
	}

	public class LocalRoation : Base<RectTransform, Vector3> {
		public override void Update(float time) {
			if(!target) return;
			target.localRotation = Quaternion.Euler(Vector3.LerpUnclamped(start, end, time));
		}
	}

	public class LocalScale : Base<RectTransform, Vector3> {
		public override void Update(float time) {
			if(!target) return;
			target.localScale = Vector3.LerpUnclamped(start, end, time);
		}
	}

	public class AnchoredPosition : Base<RectTransform, Vector2> {
		public override void Update(float time) {
			if(!target) return;
			target.anchoredPosition = Vector2.LerpUnclamped(start, end, time);
		}
	}

	public class Pivot : Base<RectTransform, Vector2> {
		public override void Update(float time) {
			if(!target) return;
			target.pivot = Vector2.LerpUnclamped(start, end, time);
		}
	}

	public class Size : Base<RectTransform, Vector2> {
		public override void Update(float time) {
			if(!target) return;
			target.sizeDelta = Vector2.LerpUnclamped(start, end, time);
		}
	}

}
