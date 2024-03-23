using System;
namespace Drafts {

	[Serializable]
	public class DraftsFunc<R> : DraftsAction {
		public R Invoke() => (R)base.Invoke();
		public DraftsFunc() => signature = $"{typeof(R).Name}";
	}

	[Serializable]
	public class DraftsFunc<A, R> : DraftsAction {
		public R Invoke(A a) => (R)base.Invoke(a);
		public DraftsFunc() => signature = $"{typeof(A).Name},{typeof(R).Name}";
	}

	[Serializable]
	public class DraftsFunc<A, B, R> : DraftsAction {
		public R Invoke(A a, B b) => (R)base.Invoke(a, b);
		public DraftsFunc() => signature = $"{typeof(A).Name},{typeof(B).Name},{typeof(R).Name}";
	}

	[Serializable]
	public class DraftsFunc<A, B, C, R> : DraftsAction {
		public object Invoke(A a, B b, C c) => (R)base.Invoke(a, b, c);
		public DraftsFunc() => signature = $"{typeof(A).Name},{typeof(B).Name},{typeof(C).Name},{typeof(R).Name}";
	}

	[Serializable]
	public class DraftsFunc<A, B, C, D, R> : DraftsAction {
		public object Invoke(A a, B b, C c, D d) => (R)base.Invoke(a, b, c, d);
		public DraftsFunc() => signature = $"{typeof(A).Name},{typeof(B).Name},{typeof(C).Name},{typeof(D).Name},{typeof(R).Name}";
	}

}
