using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Drafts {
	public partial class Updater {

		public class Routine {
			Coroutine coroutine;
			Func<IEnumerator> getIE;

			public Routine() { }
			public Routine(Func<IEnumerator> getIE) => this.getIE = getIE;

			public void Start(IEnumerator ie) => coroutine = ie.Start();
			public void Stop() { coroutine?.Stop(); coroutine = null; }
			public void Override(IEnumerator ie) => ie.Override(ref coroutine);

			public void Start() {
				if(getIE == null) throw new Exception("No Func set in the constructor");
				Start(getIE());
			}
			public void Restart() {
				if(getIE == null) throw new Exception("No Func set in the constructor");
				Override(getIE());
			}
		}
	}

	public static class ExtensionsCoroutine {
		public static Coroutine Start(this IEnumerator enumerator) 
			=> Updater.Instance.StartCoroutine(enumerator);

		public static Coroutine ContinueWith(this Coroutine routine, IEnumerator enumerator)
			=> Updater.Instance.StartCoroutine(_ContinueWith(routine, enumerator));

		public static Coroutine ContinueWith(this Coroutine routine, Action action)
			=> Updater.Instance.StartCoroutine(_ContinueWith(routine, action));

		static IEnumerator _ContinueWith(Coroutine coroutine, IEnumerator enumerator) {
			yield return coroutine;
			yield return enumerator;
		}

		static IEnumerator _ContinueWith(Coroutine coroutine, Action action) {
			yield return coroutine;
			action();
		}

		public static void Stop(this Coroutine coroutine) => Updater.Instance.StopCoroutine(coroutine);
		public static void Override(this IEnumerator enumerator, ref Coroutine coroutine) {
			if(coroutine != null) Updater.Instance.StopCoroutine(coroutine);
			coroutine = Updater.Instance.StartCoroutine(enumerator);
		}

		public static async Task StartAsync(this IEnumerator coroutine) {
			var tcs = new TaskCompletionSource<object>();
			Updater.Instance.StartCoroutine(WrapCoroutine(coroutine, tcs));
			await tcs.Task;
		}
		public static async Task StartAsync<T>(this IEnumerator<T> coroutine) {
			var tcs = new TaskCompletionSource<T>();
			Updater.Instance.StartCoroutine(WrapCoroutine(coroutine, tcs));
			await tcs.Task;
		}
		static IEnumerator WrapCoroutine(IEnumerator coroutine, TaskCompletionSource<object> tcs) {
			yield return coroutine;
			tcs.SetResult(coroutine.Current);
		}
		static IEnumerator WrapCoroutine<T>(IEnumerator<T> coroutine, TaskCompletionSource<T> tcs) {
			yield return coroutine;
			tcs.SetResult(coroutine.Current);
		}

	}
}