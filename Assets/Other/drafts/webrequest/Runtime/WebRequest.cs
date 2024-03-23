using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace Drafts.Web {

	/// <summary>No retry.</summary>
	public class WebRequest : CustomYieldInstruction, IDisposable {

		public bool offlineMode { get; private set; }
		bool? succeed;
		protected string text;
		protected byte[] data;
		protected Texture2D texture;
		public object userData;

		public UnityWebRequest UnityRequest;
		public override bool keepWaiting => !UnityRequest.isDone && !offlineMode;

		/// <summary>When calling Suceed after the request is done, this will determine if a request was sucessfull.
		/// You can use the ExeptionHandleResponse to throw specific exeption for each error type.</summary>
		public Func<UnityWebRequest, bool> CheckSuccess;
		/// <summary>Request status code when suceeded.</summary>
		public StatusCode StatusCode => offlineMode ? StatusCode.OK : (StatusCode)UnityRequest.responseCode;
		/// <summary>The request succeeded and has no protocol errors.</summary>
		public bool OK => Succeed && RequestResult == UnityWebRequest.Result.Success;
		public UnityWebRequest.Result RequestResult => offlineMode ? UnityWebRequest.Result.Success : UnityRequest.result;
		/// <summary>The request succeeded. Check StatusCode for protocol possible errors.</summary>
		public bool Succeed => succeed ??= CheckSuccess(UnityRequest);
		/// <summary>Download Handler text</summary>
		public string Text => text ??= UnityRequest.downloadHandler.text;
		/// <summary>Download Handler data</summary>
		public byte[] Data => data ??= UnityRequest.downloadHandler.data;
		/// <summary>Download Handler data converted using LoadImage.</summary>
		public Texture2D Texture => texture ??= (OK ? RequestDefault.GetTexture(UnityRequest.downloadHandler) : null);
		/// <summary>Download Handler text converted using JsonUtility.</summary>
		public R Json<R>() {
			try {
				return JsonUtility.FromJson<R>(Text);
			} catch(Exception e) {
				Debug.Log(Text);
				Debug.LogException(e);
				return default;
			}
		}

		public WebRequest(UnityWebRequest request, bool offlineMode = false) {
			this.offlineMode = offlineMode;
			UnityRequest = request;
			if(offlineMode) CheckSuccess = r => true;
			else CheckSuccess = RequestDefault.DebugResponse;
			if(!offlineMode) UnityRequest.SendWebRequest();
		}

		public WebRequest OfflineData(Func<string> getData) {
			if(!offlineMode) return this;
			text = getData();
			return this;
		}

		public WebRequest OfflineData(Func<Texture2D> getData) {
			if(!offlineMode) return this;
			texture = getData();
			return this;
		}

		/// <summary>Suceed: the value will be null on protocol error. Fail: Connection, Server, or DATA Processing error has ocured.</summary>
		public void SetCallback(RequestSuceed<string> onSuceed, RequestFailed onFail = null) => _WaitCallback(onSuceed, onFail ?? RequestDefault.OnFail).Start();
		/// <summary>Suceed: the value will be null on protocol error. Fail: Connection, Server, or DATA Processing error has ocured.</summary>
		public void SetCallback(RequestSuceed<byte[]> onSuceed, RequestFailed onFail = null) => _WaitCallback(onSuceed, onFail ?? RequestDefault.OnFail).Start();
		/// <summary>Suceed: the value will be null on protocol error. Fail: Connection, Server, or DATA Processing error has ocured.</summary>
		public void SetCallback(RequestSuceed<Texture2D> onSuceed, RequestFailed onFail = null) => _WaitCallback(onSuceed, onFail ?? RequestDefault.OnFail).Start();
		/// <summary>Suceed: the value will be null on protocol error. Fail: Connection, Server, or DATA Processing error has ocured.</summary>
		public void SetCallback<T>(RequestSuceed<T> onSuceed, RequestFailed onFail = null) => _WaitCallback(onSuceed, onFail ?? RequestDefault.OnFail).Start();

		/// <summary>Suceed: a response was received, protocol errors may have ocured. Fail: Connection, Server, or DATA Processing error has ocured.</summary>
		public void SetCallback(RequestObject<StatusCode> onSuceed, RequestFailed onFail = null) => _WaitCallback(onSuceed, onFail ?? RequestDefault.OnFail).Start();
		/// <summary>Suceed: the value will be false on protocol error. Fail: Connection, Server, or DATA Processing error has ocured.</summary>
		public void SetCallback(RequestObject<bool> onSuceed, RequestFailed onFail = null) => _WaitCallback(onSuceed, onFail ?? RequestDefault.OnFail).Start();
		/// <summary>Suceed: the value will be null on protocol error. Fail: Connection, Server, or DATA Processing error has ocured.</summary>
		public void SetCallback(RequestObject<string> onSuceed, RequestFailed onFail = null) => _WaitCallback((c, v) => onSuceed(v), onFail ?? RequestDefault.OnFail).Start();
		/// <summary>Suceed: the value will be null on protocol error. Fail: Connection, Server, or DATA Processing error has ocured.</summary>
		public void SetCallback(RequestObject<byte[]> onSuceed, RequestFailed onFail = null) => _WaitCallback((c, v) => onSuceed(v), onFail ?? RequestDefault.OnFail).Start();
		/// <summary>Suceed: the value will be null on protocol error. Fail: Connection, Server, or DATA Processing error has ocured.</summary>
		public void SetCallback(RequestObject<Texture2D> onSuceed, RequestFailed onFail = null) => _WaitCallback((c, v) => onSuceed(v), onFail ?? RequestDefault.OnFail).Start();
		/// <summary>Suceed: the value will be null on protocol error. Fail: Connection, Server, or DATA Processing error has ocured.</summary>
		public void SetCallback<T>(RequestObject<T> onSuceed, RequestFailed onFail = null) => _WaitCallback<T>((c, v) => onSuceed(v), onFail ?? RequestDefault.OnFail).Start();

		IEnumerator _WaitCallback(RequestObject<StatusCode> onSuceed, RequestFailed onFail) { using(this) { yield return this; if(Succeed) onSuceed(StatusCode); else onFail((RequestResult)UnityRequest.result, UnityRequest.error); } }
		IEnumerator _WaitCallback(RequestSuceed<string> onSuceed, RequestFailed onFail) { yield return this; if(Succeed) onSuceed(StatusCode, Text); else onFail((RequestResult)UnityRequest.result, UnityRequest.error); UnityRequest.Dispose(); }
		IEnumerator _WaitCallback(RequestSuceed<byte[]> onSuceed, RequestFailed onFail) { yield return this; if(Succeed) onSuceed(StatusCode, Data); else onFail((RequestResult)UnityRequest.result, UnityRequest.error); UnityRequest.Dispose(); }
		IEnumerator _WaitCallback(RequestSuceed<Texture2D> onSuceed, RequestFailed onFail) { yield return this; if(Succeed) onSuceed(StatusCode, Texture); else onFail((RequestResult)UnityRequest.result, UnityRequest.error); UnityRequest.Dispose(); }
		IEnumerator _WaitCallback<T>(RequestSuceed<T> onSuceed, RequestFailed onFail) { yield return this; if(Succeed) onSuceed(StatusCode, Json<T>()); else onFail((RequestResult)UnityRequest.result, UnityRequest.error); UnityRequest.Dispose(); }
		IEnumerator _WaitCallback(RequestObject<bool> onSuceed, RequestFailed onFail) { yield return this; if(Succeed) onSuceed(OK); else onFail((RequestResult)UnityRequest.result, UnityRequest.error); UnityRequest.Dispose(); }

		public void Dispose() => UnityRequest.Dispose();
	}

	/// <summary>No retry.</summary>
	public class WebRequest<T> : WebRequest {
		public WebRequest(UnityWebRequest request, bool offlineMode = false) : base(request, offlineMode) { }
		protected T result;
		public T Result => result ??= GetResult(UnityRequest.downloadHandler);
		protected virtual T GetResult(DownloadHandler dh) => Json<T>();
		public virtual WebRequest<T> OfflineData(Func<T> getData) { if(offlineMode) result = getData(); return this; }
	}

	public static class ExtensionsWebRequest {
		public static T UserData<T>(this T request, object data) where T : WebRequest {
			request.userData = data;
			return request;
		}
	}
}