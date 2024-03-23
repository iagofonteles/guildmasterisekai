using Drafts.Components;
using Drafts.Extensions;
using UnityEngine;

namespace Drafts.Samples {
	public class TypeWritterCommands {

		public abstract class Command {
			public abstract string Name { get; }
			protected TypeWritterComponent typeWritter;
			public Command(TypeWritterComponent typeWritter) => this.typeWritter = typeWritter;
			protected string[] param;
			protected int paramInt(int paramId) => int.Parse(param[paramId]);

			protected abstract void Parse();
			protected void Error(string error) => Debug.LogError($"TypeWritter Command {Name}: {error}");

			public void Execute(string[] param) {
				this.param = param;
				Parse();
			}
		}

		public class Speed : Command {
			public override string Name => "spd";
			float defaultDelay;
			float[] delays;

			public Speed(TypeWritterComponent typeWritter, float[] delays) : base(typeWritter) {
				this.delays = delays;
				defaultDelay = typeWritter.delay;
			}

			protected override void Parse() {
				if(param.Length < 2) typeWritter.delay = defaultDelay;
				else if(paramInt(1) < delays.Length)
					typeWritter.delay = delays[paramInt(1)];
				else Error($"index {paramInt(1)} out of bounds");
			}
		}
	}
}
