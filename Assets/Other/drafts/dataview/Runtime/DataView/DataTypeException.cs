namespace Drafts {
	public class DataTypeException<E> : UException {
		public DataTypeException(object o, UnityEngine.Object ctx = null) 
			: base($"Expected: {typeof(E).Name}, Got: {o.GetType().Name}", ctx) { }
	}
}