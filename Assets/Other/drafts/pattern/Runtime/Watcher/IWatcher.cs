using System;

namespace Drafts.Patterns {

	public interface IWatcher {
		event Action OnModified;
	}

	public interface IWatcher<T> {
		event Action<T> OnChanged;
	}

	public interface IStatWatcher<T> {
		event ChangedEventHandler<T> OnStatChanged;
	}

	public delegate void ChangedEventHandler<T>(object sender, ChangedEventArgs<T> e);
	public delegate void Processor<T>(object sender, object source, float current, ref T delta);

	public class ChangedEventArgs<T> {
		public readonly object Source;
		public readonly T New;
		public readonly T Old;
		CDelta delta;
		public CDelta Delta => delta ??= GetDelta();

		public ChangedEventArgs(object source, T @new, T old) {
			Source = source;
			New = @new;
			Old = old;
		}

		CDelta GetDelta() {
			if(New is int i1 && Old is int i2) return new() { value = i1 - i2 };
			if(New is float f1 && Old is float f2) return new() { value = f1 - f2 };
			return default;
		}

		public class CDelta {
			internal float value;
			public static implicit operator float(CDelta d) => d?.value ?? throw new Exception("not float");
			public static implicit operator int(CDelta d) => (int)(d?.value ?? throw new Exception("not int"));
		}
	}
}
