using UnityEngine;
namespace Drafts.DAnima {

	public class Alpha : Base<CanvasGroup, float> {
		public override void Update(float time) {
			if(!target) return;
			target.alpha = Mathf.LerpUnclamped(start, end, time);
		}
	}

	public class Fade : Base<CanvasGroup>, ISingleLineInspector {
		float ISingleLineInspector.RightPadding => 60;
		public override void Update(float time) {
			if(!target) return;
			target.alpha = Mathf.LerpUnclamped(0, 1, time);
		}
	}

	public class FadeOut : Base<CanvasGroup>, ISingleLineInspector {
		float ISingleLineInspector.RightPadding => 60;
		public override void Update(float time) {
			if(!target) return;
			target.alpha = Mathf.LerpUnclamped(1, 0, time);
		}
	}

	public class Interaction : Base<CanvasGroup> {
		public bool interactableStart, interactableEnd;
		public bool blocksRaycastStart, blocksRaycastEnd;

		public override void Subscribe(Anima anima) {
			base.Subscribe(anima);
			anima.callbacks.OnPlay.AddListener(SetInteraction);
		}
		private void SetInteraction(bool f) {
			target.interactable = f ? interactableEnd : interactableStart;
			target.blocksRaycasts = f ? blocksRaycastEnd : blocksRaycastStart;
		}
		public override void Update(float time) { }
	}

	public class Raycast : Base<CanvasGroup>, ISingleLineInspector {
		float ISingleLineInspector.RightPadding => 60;
		public override void Subscribe(Anima anima) {
			base.Subscribe(anima);
			anima.callbacks.OnPlay.AddListener(SetInteraction);
		}
		private void SetInteraction(bool f) => target.blocksRaycasts = f;
		public override void Update(float time) { }
	}
}
