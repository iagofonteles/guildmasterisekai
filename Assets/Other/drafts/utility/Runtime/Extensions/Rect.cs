using UnityEngine;

namespace Drafts {

	public static partial class RectUtility {

		public static Rect NextY(this ref Rect rect, float height) {
			var r = rect;
			r.height = height;
			rect.height -= height;
			rect.y += height;
			return r;
		}

		public static Rect NextX(this ref Rect rect, float width) {
			var r = rect;
			r.width = width;
			rect.width -= width;
			rect.x += width;
			return r;
		}
	}

	public class ControlRect {
		public Rect src, current;

		public float x => current.x;
		public float y => current.y;
		public float width => current.width;
		public float height => current.height;

		public ControlRect() { }
		public ControlRect(Rect src, int lines = 1) {
			this.src = current = src;
			current.height /= lines;
		}

		public Rect NextLine() {
			var r = current;
			current.width = src.width;
			current.y += current.height;
			current.x = src.x;
			return r;
		}

		public Rect MoveX(float width) {
			var r = current;
			r.width = Mathf.Abs(width);
			current.width -= Mathf.Abs(width);
			if(width > 0) current.x += width;
			else r.x += current.width;
			return r;
		}

		public Rect MoveY(float height) {
			var r = current;
			r.height = Mathf.Abs(height);
			current.height -= Mathf.Abs(height);
			if(height > 0) current.y += height;
			else r.y += current.height;
			return r;
		}

		public void Reset() => current = src;

		public void Indent(int space) {
			src.width -= space;
			src.x += space;
			current.width -= space;
			current.x += space;
		}
	}
}