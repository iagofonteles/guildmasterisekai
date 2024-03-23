using System;
using System.Collections.Generic;
using UnityEngine;

namespace Drafts.UI {
	public class ContextMenu {
		public string title;
		public List<Item> itens;

		public ContextMenu(List<Item> itens) : this("", itens) { }
		public ContextMenu(string title, List<Item> itens) {
			this.title = title;
			this.itens = itens;
		}

		public class Item {
			public Sprite icon;
			public string text;
			public Func<object> action;
			public bool valid;

			public Item(string text, Func<object> action, bool valid = true)
				 : this(null, text, action, valid) { }

			public Item(string text, Action action, bool valid = true)
				 : this(null, text, () => { action(); return null; }, valid) { }

			public Item(Sprite icon, string text, Action action, bool valid = true)
				 : this(icon, text, () => { action(); return null; }, valid) { }

			public Item(Sprite icon, string text, Func<object> action, bool valid = true) {
				this.icon = icon;
				this.text = text;
				this.action = action;
				this.valid = valid;
			}
		}
	}
}
