//namespace Drafts.Patterns {
//	public interface IStatWatcher<T, S> {
//		public delegate void Callback(S source, T value, T delta);

//		T Min { get; }
//		T Max { get; }
//		T Base { get; }
//		T Value { get; }

//		event Callback OnValueChanged;
//		event Callback OnBaseChanged;
//		event Callback OnMinChanged;
//		event Callback OnMaxChanged;

//		void Set(S source, T value);
//		void Add(S source, T value);
//		void Sub(S source, T value);
//		void Mult(S source, T value);
//		void Div(S source, T value);

//		void Reset(S source);

//		void ChangeMin(S source, T value);
//		void ChangeMax(S source, T value);
//		void ChangeBase(S source, T value);
//	}
//}