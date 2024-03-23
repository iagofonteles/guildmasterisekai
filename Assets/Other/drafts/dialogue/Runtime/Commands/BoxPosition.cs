using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Drafts.Dialogues.Commands {

	public class BoxPosition : Command {
		public override string Name => "/box_position";

		public override string Help { get; } = @"/box_position slot_name";

		RectTransform box;
		List<RectTransform> slots;

		public BoxPosition(DialogueController d, RectTransform box, List<RectTransform> slots) : base(d) {
			this.box = box;
			this.slots = slots;
		}

		protected override void Parse() {
			var slot = slots.FirstOrDefault(s => s.name == param[1]);
			if(!slot) Debug.Log($"box slot {param[1]} no found");
			else {
				box.anchoredPosition = slot.anchoredPosition;
				box.anchorMin = slot.anchorMin;
				box.anchorMax = slot.anchorMax;
				box.pivot = slot.pivot;
			}
		}
	}
}
