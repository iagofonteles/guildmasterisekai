using System;
using UnityEngine.Networking;
using UnityEngine;

namespace Drafts.Web {

    public static class HttpStatusCodeExtension {
        public static bool Suceed(this StatusCode code) => ((int)code) >= 200 && ((int)code) < 300;
        public static bool ProtocolError(this StatusCode code) => ((int)code) >= 400 && ((int)code) < 500;
        public static bool ServerError(this StatusCode code) => ((int)code) >= 500 && ((int)code) < 600;
    }

    public class RequestError : Exception {
        public RequestError(string message) : base(message) { }
    }
    public class ConnectionError : RequestError {
        public ConnectionError(string message) : base(message) { }
    }
    public class DataProcessingError : RequestError {
        public DataProcessingError(string message) : base(message) { }
    }
    public class InProgressError : RequestError {
        public InProgressError(string message) : base(message) { }
    }
    public class ProtocolError : RequestError {
        public int Code { get; private set; }
        public StatusCode StatusCode { get; private set; }
        public ProtocolError(int code, string message) : base(message) {
            Code = code;
            StatusCode = (StatusCode)code;
        }
    }

    public delegate void RequestFailed(RequestResult result, string error);
    public delegate void RequestSuceed<T>(StatusCode code, T result);
    public delegate void RequestObject<T>(T result);

    public static class RequestDefault {

        public static readonly RequestFailed OnFail = (r, error) => Debug.LogError($"{r}: {error}");

        //public static T GetJson<T>(DownloadHandler handler) => JsonUtility.FromJson<T>(handler.text);
        public static Texture2D GetTexture(DownloadHandler handler) { var t = new Texture2D(0, 0); t.LoadImage(handler.data); return t; }

        public static bool DebugResponse(UnityWebRequest Request) {
            string error = null;
            switch (Request.result) {
                case UnityWebRequest.Result.InProgress: error = $"request incomplete on {Request.method} {Request.url}"; break;
                case UnityWebRequest.Result.ConnectionError: error = $"connection error on {Request.method} {Request.url}\n{Request.error}"; break;
                case UnityWebRequest.Result.DataProcessingError: error = $"server error on {Request.method} {Request.url}\n{Request.error}"; break;
                case UnityWebRequest.Result.ProtocolError: Debug.LogError($"protocol error on {Request.method} {Request.url}\n{Request.error}"); break;
            }
            if (error != null) Debug.Log(error);
            return error == null;
        }

        public static bool ExeptionResponse(UnityWebRequest Request) {
            string error = null;
            switch (Request.result) {
                case UnityWebRequest.Result.InProgress: throw new Exception($"request incomplete on {Request.url}");
                case UnityWebRequest.Result.ConnectionError: throw new Exception($"connection error on {Request.url}\n{Request.error}");
                case UnityWebRequest.Result.DataProcessingError: throw new Exception($"server error on {Request.url}\n{Request.error}");
                case UnityWebRequest.Result.ProtocolError: Debug.LogError($"protocol error on {Request.url}\n{Request.error}"); break;
            }
            if (error != null) Debug.Log(error);
            return error == null;
        }

	}

}