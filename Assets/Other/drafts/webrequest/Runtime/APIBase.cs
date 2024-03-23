using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Drafts.Web {
	public abstract class APIBase : ScriptableObject {

		public string Branch { get; private set; }
		protected const string GET = "GET", POST = "POST", PUT = "PUT", PATCH = "PATCH", DELETE = "DELETE";
		public WebClient Client { get; protected set; }
		[SerializeField, ReadOnly(true)] SerializableWebClient _client;

		[SerializeField, LocalDropdown("HeaderList")] string headerList;
		IEnumerable<string> HeaderList => Client?.defaultHeaders.Select(p => $"{p.Key}: {p.Value}") ?? new string[0];

		public void Initialize() {
			Client ??= _client;
			Branch ??= _client.usedUrl;
		}

		public void SetBaseUrl(string url) => Client.baseURL = url;
		public void SetAcceptLanguage(string lang) => Client.defaultHeaders["Accept-Language"] = lang;
		public void SetAuthBearerToken(string token) => Client.defaultHeaders["Authorization"] = "Bearer " + token;
		public string GetAuthBearerToken() => Client.defaultHeaders["Authorization"].Remove(0, 7);

		public void SetAccept(string contentType) => Client.defaultHeaders["Accept"] = contentType;
		public void SetContentType(string contentType) => Client.defaultHeaders["Content-Type"] = contentType;

		public WebRequest R(string method, string path, object body = null, Dictionary<string, string> headers = null, int? timeout = null)
			=> Client.Request(method, path, body, headers, timeout);

		public WebRequest<R> R<R>(string method, string path, object body = null, Dictionary<string, string> headers = null, int? timeout = null)
			=> Client.Request<R>(method, path, body, headers, timeout);
	}
}
