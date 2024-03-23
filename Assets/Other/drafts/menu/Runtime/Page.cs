using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Drafts.Menus {

	public interface IOpenClose { }
	public interface ISetActive { }
	public interface IAnimationProvider {
		void Play(bool instant, Action callback = null);
	}

	public partial class Page : MonoBehaviour, IOpenClose, ISetActive {
		public static List<Page> instances;
		public static IReadOnlyList<Page> Instances => instances;

		public static event Action<string, bool> OnChanged;
		public static string Current { get; private set; }

		[NonSerialized] public List<Page> children = new List<Page>();

		public bool IsOpen { get; set; }
		[ContextMenu("Open")] public void Open() => Open(null);
		[ContextMenu("Close")] public void Close() => Close(null);
		[ContextMenu("Toggle")] public void Toggle() => SetActive(!IsOpen, null);
		public void SetActive(bool active) { if(active) Open(); else Close(); }
		public void SetActive(bool active, Action callback) { if(active) Open(callback); else Close(callback); }

		/// <summary>Callback after the Animation or imediatelly if waitAnimation is not checked.</summary>
		public void Open(Action callback) {
			// wait for parent to open
			if(parent) {
				parent.Open(() => {
					// wait siblings to close before open
					var current = parent.children.FirstOrDefault(p => p != this && p.IsOpen);
					if(current) current.Close(() => Set(true, callback));
					else Set(true, callback);
				});
			} else Set(true, callback);
		}

		/// <summary>Callback after the Animation or imediatelly if waitAnimation is not checked.</summary>
		public void Close(Action callback) => Set(false, callback);

		public Page Child(string name) => children.FirstOrDefault(p => p.name == name);
		public Page Sibling(string name) => parent.Child(name);

		void Set(bool open, Action callback) {
			// return imediately if already
			if(IsOpen == open) {
				callback?.Invoke();
				return;
			}

			IsOpen = open;
			CallCallbacks(open, true);

			// callback imediately if not wait
			if(!waitAnimation) {
				callback?.Invoke();
				callback = null;
			}

			// call animation
			void Callback() { CallCallbacks(open, false); callback?.Invoke(); }
			(open ? openAnimation : closeAnimation).Play(false, Callback);

			// global callback
			OnChanged?.Invoke(name, open);
			if(open) Current = name;
		}
	}
}
