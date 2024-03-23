using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Drafts.Inventory {

	/// <summary>Inventory with finite slots. Can Favorite item to reserve the slot.
	/// Favorited slots will show "Item x0" instead of removing the item from the slot.</summary>
	[Serializable]
	public class ListInventory<ITEM> : IInventory, IEnumerable<Slot<ITEM>> {
		//[SerializeField] List<Slot<ITEM>> slots = new();
		[SerializeField] List<Slot<ITEM>> slots = new();
		readonly Func<ITEM, int> maxStack; // function to extract max stack size of a specific item
		public Slot<ITEM> this[int index] => slots[index];

		public event Action<ITEM, int> OnItemChanged;
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public Slot<ITEM> SlotWith(ITEM item) => slots.FirstOrDefault(s => s.Item?.Equals(item) ?? false);	

		/// <param name="maxStack">If you dont want to specify for each item, just put "i=>99" for fixed stack size or leave it null for unlimited.</param>
		public ListInventory(Func<ITEM, int> maxStack = null) => this.maxStack = maxStack ?? (i => int.MaxValue);

		/// <summary>Return the amout that could not be inserted.</summary>
		public int Add(ITEM item, int count = 1) {
			if(count < 0) return Remove(item, -count);
			if(item == null) throw new ArgumentNullException("item cannot be null.");
			var initCount = count;

			var slot = SlotWith(item);
			if(slot == null) {
				slots.Add(slot = new(item));
				CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Add, slot, slots.Count - 1));
			}

			var v = Math.Min(maxStack(item) - slot.Amount, count);
			slot.Amount += v;
			count -= v;

			var delta = initCount - count;
			if(delta != 0) OnItemChanged?.Invoke(item, delta);
			return count;
		}

		public int Remove(ITEM item, int count = 1) {
			if(count < 0) return Add(item, -count);
			if(item == null) throw new ArgumentNullException("item cannot be null.");
			var initCount = count;

			var slot = SlotWith(item);
			if(slot == null) return count;

			var v = Math.Min(slot.Amount, count);
			slot.Amount -= v;
			count -= v;

			if(slot.IsEmpty) {
				var index = slots.IndexOf(slot);
				slots.RemoveAt(index);
				CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Remove, slot, index));
			}

			var delta = initCount - count;
			if(delta != 0) OnItemChanged?.Invoke(item, -delta);
			return count; // return the amout that could not be removed
		}

		/// <summary>If Count >= amout, return true and remove items from inventory.</summary>
		public bool Use(ITEM item, int amount = 1) { if(Count(item) < amount) return false; Remove(item, amount); return true; }
		public int Count(ITEM item) => SlotWith(item)?.Amount ?? 0;
		public int Fits(ITEM item) => maxStack(item) - Count(item);
		public bool Contains(ITEM item) => Count(item) > 0;
		/// <summary>Number of slots current in inventory.</summary>
		public int Length => slots.Count;

		public void Clear(bool clearFavorites = false) {
			for(int i = slots.Count - 1; i >= 0; i--) {
				if(clearFavorites) slots[i].Favorite = false;
				slots[i].Amount = 0;
			}
			slots.Clear();
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public void Sort(Comparer<Slot<ITEM>> comparer, bool executeUpdate) => throw new NotImplementedException();

		public IEnumerator<Slot<ITEM>> GetEnumerator() => slots.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => slots.GetEnumerator();
	}
}
