using UnityEngine;
using UnityEngine.Events;

namespace Drafts.Components {

	public class TriggerOnCondition : MonoBehaviour {
		public UnityEvent<bool> callback;
		public float compareValue;
		public string compareString;

		public void CallIf(GameObject go) { if(go) callback.Invoke(true); }
		public void CallIfNot(GameObject go) { if(!go) callback.Invoke(true); }
		public void CallIf(bool call) { if(call) callback.Invoke(true); }
		public void CallIfNot(bool notCall) { if(!notCall) callback.Invoke(true); }

		public void CallIfEquals(int value) { if(value == compareValue) callback.Invoke(true); }
		public void CallIfNotEquals(int value) { if(value != compareValue) callback.Invoke(true); }
		public void CallIfGreater(int value) { if(value > compareValue) callback.Invoke(true); }
		public void CallIfLesser(int value) { if(value < compareValue) callback.Invoke(true); }

		public void CallIfEquals(float value) { if(value == compareValue) callback.Invoke(true); }
		public void CallIfNotEquals(float value) { if(value != compareValue) callback.Invoke(true); }
		public void CallIfGreater(float value) { if(value > compareValue) callback.Invoke(true); }
		public void CallIfLesser(float value) { if(value < compareValue) callback.Invoke(true); }

		public void CallIfEquals(string value) { if(value == compareString) callback.Invoke(true); }
		public void CallIfNotEquals(string value) { if(value != compareString) callback.Invoke(true); }
		public void CallIfEquals(bool value) { if((value ? 1 : 0) == compareValue) callback.Invoke(true); }
		public void CallIfNotEquals(bool value) { if((value ? 1 : 0) != compareValue) callback.Invoke(true); }

		public void CallEquals(int value) => callback.Invoke(value == compareValue);
		public void CallNotEquals(int value) => callback.Invoke(value != compareValue);
		public void CallGreater(int value) => callback.Invoke(value > compareValue);
		public void CallGreaterEquals(int value) => callback.Invoke(value >= compareValue);
		public void CallLesser(int value) => callback.Invoke(value < compareValue);
		public void CallLesserEquals(int value) => callback.Invoke(value <= compareValue);

		public void CallEquals(float value) => callback.Invoke(value == compareValue);
		public void CallNotEquals(float value) => callback.Invoke(value != compareValue);
		public void CallGreater(float value) => callback.Invoke(value > compareValue);
		public void CallGreaterEquals(float value) => callback.Invoke(value >= compareValue);
		public void CallLesser(float value) => callback.Invoke(value < compareValue);
		public void CallLesserEquals(float value) => callback.Invoke(value <= compareValue);

		public void Call(bool value) => callback.Invoke(value);
		public void CallInverse(bool value) => callback.Invoke(!value);

		public void CallAfter(float time) => Invoke(nameof(_Call), time);
		void _Call() => callback.Invoke(true);
	}
}
