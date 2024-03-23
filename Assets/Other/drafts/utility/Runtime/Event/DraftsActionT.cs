using System;
namespace Drafts {

	[Serializable]
	public class DraftsAction<A> : DraftsAction {
		//new public void Invoke() => base.Invoke();
		public void Invoke(A a) => base.Invoke(a);
		public DraftsAction() => signature = $"{typeof(A).Name},";
	}

	[Serializable]
	public class DraftsAction<A, B> : DraftsAction {
		//new public void Invoke() => base.Invoke();
		public void Invoke(A a, B b) => base.Invoke(a, b);
		public DraftsAction() => signature = $"{typeof(A).Name},{typeof(B).Name},";
	}

	[Serializable]
	public class DraftsAction<A, B, C> : DraftsAction {
		//new public void Invoke() => base.Invoke();
		public void Invoke(A a, B b, C c) => base.Invoke(a, b, c);
		public DraftsAction() => signature = $"{typeof(A).Name},{typeof(B).Name},{typeof(C).Name},";
	}

	[Serializable]
	public class DraftsAction<A, B, C, D> : DraftsAction {
		//new public void Invoke() => base.Invoke();
		public void Invoke(A a, B b, C c, D d) => base.Invoke(a, b, c, d);
		public DraftsAction() => signature = $"{typeof(A).Name},{typeof(B).Name},{typeof(C).Name},{typeof(D).Name},";
	}
}