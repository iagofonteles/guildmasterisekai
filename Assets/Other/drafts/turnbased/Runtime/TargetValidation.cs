using System;
using System.Collections;
using System.Linq;
using Drafts.Linq;
namespace Drafts.TurnBased {

	public enum TargetType { Self, Partner, Ally, Enemy, Any, Ohter }

	public interface ITargetGroup {
		IEnumerable All { get; }
		Func<int> Count { get; }
		bool HasChoice { get; }
		object TargetType { get; }

		Func<object, bool> Validate { get; }
		Func<bool> HasEnough { get; }
		Func<IEnumerable, bool> IsValid { get; }

		IEnumerable Valid => All.Where(Validate);
	}

	public class TargetGroup : ITargetGroup {
		public virtual IEnumerable All { get; private set; }
		public virtual Func<int> Count { get; private set; }
		public bool HasChoice { get; private set; }
		public virtual object TargetType { get; private set; }

		public Func<object, bool> Validate { get; private set; }
		public Func<bool> HasEnough { get; private set; }
		public virtual Func<IEnumerable, bool> IsValid { get; private set; }
		
		public IEnumerable Valid => All.Where(Validate);

		protected TargetGroup() { }
		public TargetGroup(IEnumerable all, Func<int> count, bool hasChoice, object targetType,
		   Func<object, bool> validate, Func<bool> hasEnough, Func<IEnumerable, bool> isValid) {
			All = all;
			Count = count;
			HasChoice = hasChoice;
			TargetType = targetType;
			Validate = validate ?? Always;
			HasEnough = hasEnough;
			IsValid = isValid;
		}

		public static TargetGroup Fixed(object tgt, Func<object, bool> validate = null)
			=> Fixed(Enumerable.Repeat(tgt, 1), validate);

		public static TargetGroup Fixed(IEnumerable all, Func<object, bool> validate = null) {
			var g = new TargetGroup();
			g.All = all ?? throw new ArgumentNullException();
			g.Count = all.Count;
			g.HasChoice = false;
			g.TargetType = null;
			g.Validate = validate ?? Always;
			g.HasEnough = g.HasEnoughFixed;
			g.IsValid = all.SameElements;
			return g;
		}

		public static TargetGroup AllValid(IEnumerable all, Func<object, bool> validate = null) {
			var g = new TargetGroup();
			g.All = all ?? throw new ArgumentNullException();
			g.Count = g.Valid.Count;
			g.HasChoice = false;
			g.TargetType = null;
			g.Validate = validate ?? Always;
			g.HasEnough = g.Valid.Any;
			g.IsValid = g.IsValidAll;
			return g;
		}

		public static TargetGroup UpTo(IEnumerable all, Func<object, bool> validate = null, int count = int.MaxValue, object type = default) {
			var g = new TargetGroup();
			g.All = all ?? throw new ArgumentNullException();
			g.Count = () => Math.Min(count, g.Valid.Count());
			g.HasChoice = true;
			g.TargetType = type;
			g.Validate = validate ?? Always;
			g.HasEnough = g.Valid.Any;
			g.IsValid = g.IsValidUpTo;
			return g;
		}

		public static TargetGroup Exactly(IEnumerable all, int count, Func<object, bool> validate = null, object type = default) {
			var g = new TargetGroup();
			g.All = all ?? throw new ArgumentNullException();
			g.Count = () => count;
			g.HasChoice = true;
			g.TargetType = type;
			g.Validate = validate ?? Always;
			g.HasEnough = g.Valid.Any;
			g.IsValid = g.IsValidCount;
			return g;
		}

		public static TargetGroup One(IEnumerable all, Func<object, bool> validate = null, object type = default) {
			var g = new TargetGroup();
			g.All = all ?? throw new ArgumentNullException();
			g.Count = ReturnOne;
			g.HasChoice = true;
			g.TargetType = type;
			g.Validate = validate ?? Always;
			g.HasEnough = g.Valid.Any;
			g.IsValid = g.IsValidCount;
			return g;
		}

		public static TargetGroup None() => new() {
			All = Enumerable.Empty<object>(),
			Count = ReturnZero,
			HasChoice = false,
			TargetType = null,
			Validate = Never,
			HasEnough = Always,
			IsValid = IsEmpty
		};

		bool IsValidAll(IEnumerable ie) => ie.All(Validate);
		bool HasEnoughFixed() => Valid.Count() == Count();
		bool IsValidUpTo(IEnumerable ie) => ie.Count() <= Count() && ie.All(Validate);
		bool IsValidCount(IEnumerable targets) => targets.Count() == Count() && targets.All(Validate);

		static int ReturnOne() => 1;
		static int ReturnZero() => 0;
		static bool IsEmpty(IEnumerable ie) => !ie.Any();
		static bool Always() => true;
		static bool Always(object _) => true;
		static bool Never(object _) => false;
	}

	public static class GeneralizeFunc {
		public static Func<object, A> Generalize<T, A>(this Func<T, A> f) => o => o is T v ? f(v) : default;
	}
}

