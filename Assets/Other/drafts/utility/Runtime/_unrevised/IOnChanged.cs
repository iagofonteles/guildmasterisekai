using System;

namespace Drafts {
	public interface IOnChanged {
		event Action OnChanged;
	}

	[Obsolete]
	public interface IOnChanged<T> {
		event Action<T> OnChanged;
	}

}