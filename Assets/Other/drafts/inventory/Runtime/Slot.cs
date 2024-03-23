using UnityEngine;
using System;

namespace Drafts.Inventory {

	public interface ISlot {
		object Item { get; }
		int Amount { get; }
		bool Favorite { get; }
		event Action<int> OnChanged;
	}

	/// <summary>Slot containing item, count and favorite flag for IInventory.</summary>
	[Serializable]
	public class Slot<TItem> : ISlot {
		[SerializeField] int amount;
		[SerializeField] TItem item;
		[SerializeField] bool favorite;
		/// <summary>Delta count. When delta = 0, the favorite or item has changed.</summary>
		public event Action<int> OnChanged;

		/// <summary>Actual item on this inventory slot.</summary>
		public TItem Item {
			get => item;
			internal set {
				item = value;
				amount = 0;
				OnChanged?.Invoke(0);
			}
		}
		/// <summary>Quanty of the item.</summary>
		public virtual int Amount {
			get => amount;
			internal set {
				if(amount == value) return;
				var delta = value - amount;
				amount = value;
				if(IsEmpty) item = default;
				OnChanged?.Invoke(delta);
			}
		}
		/// <summary>Favorited itens are suposed the ocupy a slot even when Count is 0.</summary>
		public bool Favorite { get => favorite; set { favorite = value; OnChanged?.Invoke(0); } }
		/// <summary>Count is 0 and not favorited.</summary>
		public bool IsEmpty => Amount == 0 && !Favorite;

		object ISlot.Item => Item;

		public Slot(TItem item = default, int amount = 0, bool favorite = false) {
			this.item = item;
			this.amount = amount;
			this.favorite = favorite;
		}
	}

	/// <summary>Reflect the current amount of a given item in on inventory. OnRemove and OnChange are called too.</summary>
	[Serializable]
	public class MirrorItem<T> : ISlot {
		SlotInventory<T> Inventory { get; }
		[SerializeField] T item;
		[SerializeField] int amount;
		[SerializeField] bool favorite;

		public T Item { get => item; set { item = value; amount = Inventory.Count(value); OnChanged?.Invoke(0); } }
		public int Amount => amount;
		public bool Favorite { get => favorite; set { favorite = value; OnChanged?.Invoke(0); } }
		public event Action<int> OnChanged;
		object ISlot.Item => Item;

		public MirrorItem(SlotInventory<T> inventory, T item) {
			amount = inventory.Count(item);
			Inventory.OnItemChanged += Mirror;
		}

		~MirrorItem() => Inventory.OnItemChanged -= Mirror;

		void Mirror(T item, int delta) {
			if(!item.Equals(Item)) return;
			amount += delta;
			OnChanged?.Invoke(delta);
		}
	}
}
