//using Drafts.Extensions;
//using System;
//using Unity.Collections;
//using UnityEngine;
//using UnityEngine.Networking;

//namespace Drafts.Web {

//	public class OfflineDownloadHandler : DownloadHandlerScript {
//		protected new string text;
//		public OfflineDownloadHandler(string text) => this.text = text;
//		protected override float GetProgress() => 1;
//		protected override byte[] GetData() => throw new AccessViolationException();
//		protected override string GetText() => text;
//		protected override void ReceiveContentLengthHeader(ulong contentLength) { }
//		protected override bool ReceiveData(byte[] data, int dataLength) => true;
//		//protected override NativeArray<byte> GetNativeData() => throw new NotImplementedException();
//		protected override void CompleteContent() { }
//	}

//	public class OfflineDownloadHandler<T> : OfflineDownloadHandler {
//		protected new byte[] data;
//		public OfflineDownloadHandler(T data) : base(JsonUtility.ToJson(data)) => this.data = data.GetBytes();
//		protected override byte[] GetData() => data;
//	}

//}