using System;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace Drafts {
	public class UException : Exception {

		public UException(string message, UnityEngine.Object context = null) : base(message)
			=> Debug.LogException(this, context);

		public UException(Exception innerException, UnityEngine.Object context = null) 
			: base(innerException.Message, innerException)
			=> Debug.LogException(this, context);

		//public UException(Exception e, UnityEngine.Object context = null) : base("", e)
		//	=> Debug.LogException(e, context);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Catch<T>(Func<T> getValue, UnityEngine.Object ctx) {
			try {
				return getValue();
			} catch(Exception e) {
				Debug.LogException(e, ctx);
				return default;
			}
		}

	}
}