using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Drafts.DAnima {

	public class Chain : Base<Anima>, ISingleLineInspector {
		float ISingleLineInspector.RightPadding => 60;
		public override void Update(float time) {
			time = target.curve.Evaluate(time);
			foreach(var m in target.modules)
				m.Update(time);
		}
	}

	public class Cascade : Base<Transform> {
		public float delay;
		Anima anima;
		List<(Anima a, float s, float e)> children;

		public override void Subscribe(Anima anima) {
			base.Subscribe(anima);
			this.anima = anima;

			var go = target.gameObject;
			var mono = go.GetComponent<MonoCallback>() ?? go.AddComponent<MonoCallback>();
			mono.OnChildrenChanged += Reset;
			Reset();
		}

		public override void Update(float time) {
			if(!target) return;
			Reset();
			Setup();

			if(children == null) return;
			foreach(var c in children) c.a.SetProgress(
				Mathf.InverseLerp(c.s, c.e, time * anima.duration));
		}

		void Setup() {
			if(!Application.isPlaying) Reset();
			if(children != null) return;
			if(!anima) anima = target.GetComponentInParent<Anima>();
			if(!anima) return;

			var c = target.GetComponentsInChildren<Anima>();

			children = new(c.Where(c => c != anima).Select((c, i) => {
				var start = i * delay;
				var end = (start + c.duration);
				return (c, start, end);
			}));

			anima.duration = children.Max(c => c.e);
		}

		void Reset() => children = null;
	}
}
