using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using System;
using System.Linq;

namespace Drafts.Web {

	/// <summary>Used to configure headers in the inspector. Cast to the actual WebClient on initialization.</summary>
	[Serializable]
	public class SerializableWebClient {
		[Serializable]
		public class StrPair {
			public string key;
			public string value;
			public StrPair(string k, string v) { key = k; value = v; }
			public static implicit operator StrPair((string k, string v) tuple) => new StrPair(tuple.k, tuple.v);
		}

		[ReadOnly(true), LocalDropdown("sources")] public string usedUrl;
		[ReadOnly(true)] public StrPair[] baseUrls = new StrPair[] { 
			("localhost", "localhost"),
			("hemolog", "https://mysite/api/hemolog") 
		};
		[ReadOnly(true)] public int defaultTimeout = 30;
		[ReadOnly(true)] public int maxRetry = 10;
		[ReadOnly(true)] public bool offlineMode;

		[ReadOnly(true)]
		public List<StrPair> defaultHeaders = new List<StrPair>() {
			("Accept-Language", "en-US"),
			("Authorization", "Bearer "),
			("Accept", "application/json"),
			("Content-Type", "application/json"),
		};

		public static implicit operator WebClient(SerializableWebClient c) => new WebClient() {
			defaultHeaders = c.defaultHeaders.ToDictionary(p => p.key, p => p.value),
			baseURL = c.baseUrls.First(p => p.key == c.usedUrl).value,
			defaultTimeout = c.defaultTimeout,
			maxRetry = c.maxRetry,
			offlineMode = c.offlineMode,
		};

		IEnumerable<string> sources => baseUrls?.Select(p => p.key);
	}

	public class WebClient {
		public string baseURL;
		public Dictionary<string, string> defaultHeaders = new Dictionary<string, string>();
		public int defaultTimeout = 30;
		public int maxRetry = 10;
		public bool offlineMode;

		public WebClient() { }

		public WebClient(string baseURL, Dictionary<string, string> defaultHeaders = null, int defaultTimeout = 10, int maxRetry = 10) {
			this.baseURL = baseURL;
			this.defaultHeaders = defaultHeaders;
			this.defaultTimeout = defaultTimeout;
			this.maxRetry = maxRetry;
		}

		public WebRequest Request(string method, string path, object body = null, Dictionary<string, string> headers = null, int? timeout = null) {
			var ureq = Request(method, baseURL + path, body, headers ?? defaultHeaders, timeout ?? defaultTimeout);
			return new WebRequest(ureq, offlineMode);
		}

		public WebRequest<T> Request<T>(string method, string path, object body = null, Dictionary<string, string> headers = null, int? timeout = null) {
			var ureq = Request(method, baseURL + path, body, headers ?? defaultHeaders, timeout ?? defaultTimeout);
			return new WebRequest<T>(ureq, offlineMode);
		}

		/// <summary>Ignore default _values of base URL and headers.</summary>
		public WebRequest<T> Raw<T>(string method, string fullURL, object body = null, Dictionary<string, string> headers = null, int? timeout = null) {
			var ureq = Request(method, fullURL, body, headers, timeout ?? defaultTimeout);
			var req = new WebRequest<T>(ureq, offlineMode);
			return req;
		}

		/// <summary>Ignore default _values of base URL and headers.</summary>
		public WebRequest Raw(string method, string fullURL, object body = null, Dictionary<string, string> headers = null, int? timeout = null) {
			var ureq = Request(method, fullURL, body, headers, timeout ?? defaultTimeout);
			var req = new WebRequest(ureq, offlineMode);
			return req;
		}

		/// <summary>Create an UnityWebRequest with custom configuration.</summary>
		public static UnityWebRequest Request(string method, string url, object body, Dictionary<string, string> headers, int timeout = 10) {
			var uploader = body == null ? null : new UploadHandlerRaw(Encoding.UTF8.GetBytes(
				body is string s ? s : JsonUtility.ToJson(body))); // body
			if(uploader != null) uploader.contentType = "application/json";

			var req = new UnityWebRequest(url, method, new DownloadHandlerBuffer(), uploader);

			if(headers != null)
				foreach(var h in headers)
					req.SetRequestHeader(h.Key, h.Value);

			req.timeout = timeout;

			return req;
		}

	}
}
