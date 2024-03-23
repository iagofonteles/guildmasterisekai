using System;
using UnityEngine;
namespace Drafts.DAnima {

	public class Move : Base<RectTransform> {
		public enum Mode {
			FromLeft,
			FromRight,
			FromTop,
			FromBotton,
			FromCenter,
		}
		public Mode mode;

		(Vector2 min, Vector2 max) values;

		public override void Subscribe(Anima anima) {
			base.Subscribe(anima);
			values = GetValues();
		}

		public override void Update(float time) {
			if(!target) return;
#if UNITY_EDITOR
			if(!Application.isPlaying) values = GetValues();
#endif
			target.anchorMin = Vector2.LerpUnclamped(values.min, Vector2.zero, time);
			target.anchorMax = Vector2.LerpUnclamped(values.max, Vector2.one, time);
		}

		private (Vector2, Vector2) GetValues() => mode switch {
			Mode.FromLeft => (new(-1, 0), new(0, 1)),
			Mode.FromRight => (new(1, 0), new(2, 1)),
			Mode.FromTop => (new(0, 1), new(1, 2)),
			Mode.FromBotton => (new(0, -1), new(1, 0)),
			Mode.FromCenter => (new(.5f, .5f), new(.5f, .5f)),
			_ => throw new NotImplementedException(),
		};
	}
}
