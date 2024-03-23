using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Drafts.Dialogues.Commands {

	public class ImageSlot : Command {
		public override string Name => "/image_slot";

		public override string Help { get; } = @"/image_slot slot_name image_name
the found image_slot will be enabled and sprite set to image_name
any invalid image_name will clear the slot";

		Image[] imageSlots;
		Sprite[] imageDB;

		public ImageSlot(DialogueController d, Image[] slots, Sprite[] db) : base(d) {
			imageSlots = slots;
			imageDB = db;
			DLG.OnEnd += Reset;
			Reset();
		}

		void Reset() {
			foreach(var s in imageSlots)
				s.enabled = false;
		}

		protected override void Parse() {
			var img = imageSlots.FirstOrDefault(i => i.name == param[1]);
			var spr = imageDB.FirstOrDefault(i => i.name == param[2]);
			if(!img) Debug.Log($"image slot {param[1]} not found");
			if(!spr) Debug.Log($"sprite {param[2]} not found");

			img.sprite = spr;
			img.enabled = spr;
		}
	}
}
