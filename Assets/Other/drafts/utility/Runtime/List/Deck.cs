using System.Collections.Generic;
using UnityEngine;
using Drafts.Extensions;
using System;
using System.Linq;
using URand = UnityEngine.Random;
using System.Collections;

//namespace DraftsHidden {[Serializable] public class DeckBase { } }

namespace Drafts.CardGame {

	//	/// <summary>Add _values to the list then pop the same amount.</summary>
	//	public static List<T> Mulligan<T>(this List<T> l, List<T> values) { l.Shuffle(values); return l.PopMany(values.Count); }
	//	/// <summary>Reorganize elements in a random order.</summary>
	//	public static void Shuffle<T>(this List<T> list) => list.Shuffle(list);
	//	/// <summary>Reorganize elements in a random order.</summary>
	//	public static void Shuffle<T>(this List<T> list, IEnumerable<T> values) => list.Shuffle(list, values);
	//}

	/// <summary>
	/// Treated like a Stack: index 0 is bottom. Draw from top.
	/// Draw functions never throw errors but can return less then specified amount, check result.Count.
	/// Subscribe to events to sync UI with better performance, all events occurs after the operation.
	/// Based on System.Collections.Generic.List<T>
	/// </summary>
	[Serializable, Obsolete("", true)]
	public class Deck<T> : IEnumerable<T> {

		#region Fields
		[SerializeField] List<T> list = new List<T>();
		/// <summary>Number of elements in deck.</summary>
		public int Count => list.Count;
		/// <summary>Get elements at index.</summary>
		public T this[int index] => list[index];

		/// <summary></summary>
		public Deck() { }
		/// <summary></summary>
		public Deck(IEnumerable<T> collection) => list = new List<T>(collection);
		#endregion

		#region Events
		/// <summary>A list of indexes that was updated, use SetDirty to manually call it.</summary>
		public event Action<IEnumerable<int>> OnChange = i => { };
		/// <summary>A list of elements inserted at given index. use IEnumerable.Count if you need the length.</summary>
		public event Action<int, IEnumerable<T>> OnAdd = (i, c) => { };
		/// <summary>A list of elements removed from given index onward. use IEnumerable.Count if you need the length.</summary>
		public event Action<int, IEnumerable<T>> OnRemove = (i, c) => { };
		/// <summary>The two swapped indexes. Acess those indexes if you need the elements.</summary>
		public event Action<int, int> OnSwap = (a, b) => { };
		/// <summary>After Shuffle has been called.</summary>
		public event Action OnShuffle = () => { };
		#endregion

		#region Get
		/// <summary>Get elements from given position going down.</summary>
		public List<T> PeekAt(int index, int amount) => list.GetRange(index - amount + 1, amount);
		/// <summary>Get elements from top.</summary>
		public List<T> Peek(int amount) => PeekAt(Count - 1, amount);
		/// <summary>Get elements from bottom.</summary>
		public List<T> PeekBottom(int amount) => PeekAt(amount - 1, amount);

		/// <summary>Get one element from given position.</summary>
		public T PeekAt(int index) => PeekAt(index, 1).FirstOrDefault();
		/// <summary>Get one element from top.</summary>
		public T Peek() => Peek(1).FirstOrDefault();
		/// <summary>Get one element from bottom.</summary>
		public T PeekBottom() => PeekBottom(1).FirstOrDefault();
		#endregion

		#region Add
		/// <summary>Call OnAdd. Insert one or more cards at the given position. First added go to bottom.</summary>
		public void AddAt(int index, params T[] cards) { list.InsertRange(index, cards); OnAdd(index, cards); }
		/// <summary>Call OnAdd. Insert one or more cards at the given position. First added go to bottom.</summary>
		public void AddAt(int index, IEnumerable<T> cards) { list.InsertRange(index, cards); OnAdd(index, cards); }
		/// <summary>Call OnAdd. Insert one or more cards at bottom (index 0). First added go to bottom.</summary>
		public void AddBottom(params T[] cards) => AddAt(0, cards);
		/// <summary>Call OnAdd. Insert one or more cards at bottom (index 0). First added go to bottom.</summary>
		public void AddBottom(IEnumerable<T> cards) => AddAt(0, cards);
		/// <summary>Call OnAdd. Insert one or more cards at top (index Count-1). First added go to bottom.</summary>
		public void AddTop(params T[] cards) => AddAt(Math.Max(0, Count - 1), cards);
		/// <summary>Call OnAdd. Insert one or more cards at top (index Count-1). First added go to bottom.</summary>
		public void AddTop(IEnumerable<T> cards) => AddAt(Math.Max(0, Count - 1), cards);
		#endregion

		#region Remove
		/// <summary>Call OnRemove. Removes all elements.</summary>
		public void Clear() => Draw(Count);

		public bool Remove(T item) {
			var ret = list.Remove(item);
			if(ret) OnRemove?.Invoke(1, new[] { item });
			return ret;
		}

		/// <summary>Call OnRemove. Remove elements from given position going down.</summary>
		public List<T> DrawAt(int index, int amount) {
			amount = Math.Min(amount, Count);
			index = Mathf.Clamp(index, amount - 1, Count - 1);
			if(amount == 0) return new List<T>();

			var ret = list.PopMany(index + 1 - amount, amount);
			OnRemove(index, ret);
			return ret;
		}
		/// <summary>Call OnRemove. Remove top-most elements.</summary>
		public List<T> Draw(int amount) => DrawAt(Count - 1, amount);
		/// <summary>Call OnRemove. Remove bottom-most elements.</summary>
		public List<T> DrawBottom(int amount) => DrawAt(amount - 1, amount);
		/// <summary>Call OnRemove. Remove random elements.</summary>
		public List<T> DrawRandom(int amount) => DrawAt(URand.Range(amount - 1, Count), amount);

		/// <summary>Call OnRemove. Remove one element from given position going down.</summary>
		public T DrawAt(int index) => DrawAt(index, 1).FirstOrDefault();
		/// <summary>Call OnRemove. Remove one element from top.</summary>
		public T Draw() => Draw(1).FirstOrDefault();
		/// <summary>Call OnRemove. Remove one element from bottom.</summary>
		public T DrawBottom() => DrawBottom(1).FirstOrDefault();
		/// <summary>Call OnRemove. Remove one random element.</summary>
		public T DrawRandom() => DrawRandom(1).FirstOrDefault();

		/// <summary>Call OnRemove. Remove one specific element.</summary>
		public T Draw(Predicate<T> predicate) {
			var i = list.FindIndex(predicate);
			return i < 0 ? default : DrawAt(i);
		}
		/// <summary>Call OnRemove many times. Remove many specific elements.</summary>
		public List<T> Draw(Predicate<T> predicate, int amount) {
			var ids = list.IndexesOf(predicate).Take(amount).Reverse();
			return ids.Select(i => DrawAt(i)).ToList();
		}

		/// <summary>Call OnRemove. Same as Draw(n).</summary>
		public bool Draw(int amount, out List<T> cards) {
			cards = Draw(amount);
			return cards.Count == amount;
		}
		/// <summary>Call OnRemove. Same as Draw().</summary>
		public bool Draw(out T card) {
			card = Draw();
			return !card.Equals(default);
		}

		/// <summary>Call OnRemove many times. Remove all specific elements.</summary>
		public List<T> DrawAll(Predicate<T> predicate) => Draw(predicate, Count);
		/// <summary>Call OnRemove. Removes all elements.</summary>
		public List<T> DrawAll() => Draw(Count);

		#endregion

		#region Move
		/// <summary>Call OnChange.</summary>
		public void SetDirty(params T[] elements) => OnChange(elements.Select(e => list.IndexOf(e)));
		/// <summary>Call OnChange.</summary>
		public void SetDirty(params int[] indexes) => OnChange(indexes);
		/// <summary>Call OnChange.</summary>
		public void SetDirty(IEnumerable<int> indexes) => OnChange(indexes);
		/// <summary>Call OnChange.</summary>
		public void SetDirtyAll() { OnChange(list.Select((c, i) => i).ToList()); }

		/// <summary>Call OnChange with all indexes. Reorganize elements in a random order.</summary>
		public void Shuffle(params T[] cards) => Shuffle((IEnumerable<T>)cards);
		/// <summary>Call OnChange with all indexes. Add cards then reorganize elements in a random order.</summary>
		public void Shuffle(IEnumerable<T> cards) { list.Shuffle(cards); SetDirtyAll(); OnShuffle(); }

		/// <summary>Call OnSwap many times, for animation purpose.</summary>
		public void ShuffleSwapping() { for(var n = 0; n < Count; n++) Swap(n, URand.Range(n, Count)); OnShuffle(); }
		/// <summary>Call OnSwap many times, for animation purpose. Same as Shuffle.</summary>
		public void ShuffleSwapping(IEnumerable<T> cards) { list.AddRange(cards); ShuffleSwapping(); }
		/// <summary>Call OnChange with all indexes. Add _values to the list, shuffle then pop the same amount added.</summary>
		public List<T> Mulligan(List<T> cards) { Shuffle(cards); return Draw(cards.Count); }
		/// <summary>Call OnSwap. for animation purpose.</summary>
		public void Swap(int index1, int index2) { list.Swap(index1, index2); OnSwap(index1, index2); }

		/// <summary>Call OnChange with all indexes. Reorganize elements using default IComparer.</summary>
		public void Sort() { list.Sort(); SetDirtyAll(); }
		/// <summary>Call OnChange with all indexes. Reorganize elements given comparer.</summary>
		public void Sort(IComparer<T> comparer) { list.Sort(comparer); SetDirtyAll(); }
		/// <summary>Call OnChange with all indexes. Reorganize elements using comparison.</summary>
		public void Sort(Comparison<T> comparison) => list.Sort(comparison);

		/// <summary>Call OnChange with all indexes.</summary>
		public void Reverse() { list.Reverse(); SetDirtyAll(); }
		#endregion

		#region Unmanaged
		/// <summary>WARNING: Has no callback events. modify the list directry.</summary>
		public void UnmanagedInsert(int index, IEnumerable<T> items) => list.InsertRange(index, items);
		/// <summary>WARNING: Has no callback events. modify the list directry.</summary>
		public List<T> UnmanagedRemove(int index, int amount) {
			var ret = list.GetRange(index, amount);
			list.RemoveRange(index, amount);
			return ret;
		}
		#endregion

		public IEnumerator<T> GetEnumerator() => list.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
	}

	//[Serializable]
	//public class Zone<T> where T : class {
	//	T[] slots;
	//	public T[] Slots => slots;
	//	public T this[int index] => slots[index];

	//	public int Lenght => slots.Length;
	//	public int FreeSlots => slots.Count(s => s == null);
	//	public int Occupied => Lenght - FreeSlots;
	//	public int FirstFree => slots.IndexOf(null);

	//	public event Action<int> OnChange = i => { };
	//	public event Action<int, T> OnAdd = (i, c) => { };
	//	public event Action<int, T> OnRemove = (i, c) => { };
	//	public event Action<int, int> OnSwap = (a, b) => { };

	//	public Zone(int slots) => this.slots = new T[slots];

	//	public int FindSlot(T card) => slots.IndexOf(card);

	//	#region Operations
	//	/// <summary>Return false if specified index has a value or there is no free slots.</summary>
	//	public bool Insert(T card, int? index = null) {
	//		var i = index ?? FirstFree;
	//		if(i < 0 || (slots[i] != null)) return false;
	//		slots[i] = card;
	//		OnAdd(i, card);
	//		return true;
	//	}

	//	/// <summary></summary>
	//	public bool Remove(T card) {
	//		var i = slots.IndexOf(card);
	//		return i < 0 ? false : Remove(i);
	//	}
	//	public bool Remove(int index) {
	//		if(slots[index] == null) return false;
	//		var old = slots[index];
	//		slots[index] = null;
	//		OnRemove(index, old);
	//		return true;
	//	}

	//	public void Clear() {
	//		for(int i = 0; i < slots.Length; i++) {
	//			var old = slots[i];
	//			slots[i] = null;
	//			OnRemove(i, old);
	//		}
	//	}

	//	public void Swap(int a, int b) {
	//		var old = slots[a];
	//		slots[a] = slots[b];
	//		slots[b] = old;
	//		OnSwap(a, b);
	//	}

	//	/// <summary>WARNING. This doe not call any events, you need to repaint manually.</summary>
	//	public void Sort() {
	//		Array.Sort(slots);
	//		for(int i = 0; i < slots.Length; i++)
	//			OnChange(i);
	//	}

	//	#endregion
	//}
}
