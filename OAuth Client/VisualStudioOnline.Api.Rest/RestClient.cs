using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace VisualStudioOnline.Api.Rest
{
    public struct VsoAPI
    {
        public const string Preview1 = "1.0-preview.1";
        public const string Preview2 = "1.0-preview.2";
        public const string Version1 = "1.0";
    }

    /// <summary>
    /// Base class for TFS subsystem REST API client
    /// </summary>
    public abstract class VsoRestClient
    {
        protected const string JSON_MEDIA_TYPE = "application/json";
        protected const string JSON_PATCH_MEDIA_TYPE = "application/json-patch+json";
        protected const string HTML_MEDIA_TYPE = "text/html";

        private string _rootUrl;
        private IHttpRequestHeaderFilter _authProvider;

        protected abstract string SubSystemName
        {
            get;
        }

        protected abstract string ApiVersion
        {
            get;
        }

        public VsoRestClient(string rootUrl, IHttpRequestHeaderFilter authProvider)
        {
            _rootUrl = rootUrl;
            _authProvider = authProvider;
        }

        #region GET operation
        protected async Task<string> GetResponse(string path, string projectName = null)
        {
            return await GetResponse(path, new Dictionary<string, object>(), projectName);
        }

        protected async Task<string> GetResponse(string path, IDictionary<string, object> arguments, string projectName = null, string mediaType = JSON_MEDIA_TYPE)
        {
            using (HttpClient client = GetHttpClient(mediaType))
            {
                using (HttpResponseMessage response = client.GetAsync(ConstructUrl(projectName, path, arguments)).Result)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    CheckResponse(response, responseBody);

                    return responseBody;
                }
            }
        }
        #endregion

        #region POST operation
        protected async Task<string> PostResponse(string path, object content, string projectName = null)
        {
            return await PostResponse(path, new Dictionary<string, object>(), content, projectName);
        }

        protected async Task<string> PostResponse(string path, IDictionary<string, object> arguments, object content, string projectName = null, string mediaType = JSON_MEDIA_TYPE)
        {
            var httpContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, mediaType);

            using (HttpClient client = GetHttpClient(mediaType))
            {
                using (HttpResponseMessage response = client.PostAsync(ConstructUrl(projectName, path, arguments), httpContent).Result)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    CheckResponse(response, responseBody);

                    return responseBody;
                }
            }
        } 
        #endregion

        #region PATCH operation
        protected async Task<string> PatchResponse(string path, object content, string projectName = null, string mediaType = JSON_PATCH_MEDIA_TYPE)
        {
            return await PatchResponse(path, new Dictionary<string, object>(), content, projectName, mediaType);
        }

        protected async Task<string> PatchResponse(string path, IDictionary<string, object> arguments, object content, string projectName = null, string mediaType = JSON_PATCH_MEDIA_TYPE)
        {
            var httpContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, mediaType);

            using (HttpClient client = GetHttpClient(mediaType))
            {
                using (HttpResponseMessage response = client.PatchAsync(ConstructUrl(projectName, path, arguments), httpContent).Result)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    CheckResponse(response, responseBody);

                    return responseBody;
                }
            }
        } 
        #endregion

        #region PUT operation
        protected async Task<string> PutResponse(string path, object content, string projectName = null, string mediaType = JSON_MEDIA_TYPE)
        {
            return await PutResponse(path, new Dictionary<string, object>(), content, projectName, mediaType);
        }

        protected async Task<string> PutResponse(string path, IDictionary<string, object> arguments, object content, string projectName = null, string mediaType = JSON_MEDIA_TYPE)
        {
            var httpContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, mediaType);

            using (HttpClient client = GetHttpClient(mediaType))
            {
                using (HttpResponseMessage response = client.PutAsync(ConstructUrl(projectName, path, arguments), httpContent).Result)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    CheckResponse(response, responseBody);

                    return responseBody;
                }
            }
        } 
        #endregion

        #region DELETE operation
        protected async Task<string> DeleteResponse(string path, string projectName = null)
        {
            return await DeleteResponse(path, new Dictionary<string, object>(), projectName);
        }

        protected async Task<string> DeleteResponse(string path, IDictionary<string, object> arguments, string projectName = null)
        {
            using (HttpClient client = GetHttpClient())
            {
                using (HttpResponseMessage response = client.DeleteAsync(ConstructUrl(projectName, path, arguments)).Result)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    CheckResponse(response, responseBody);

                    return responseBody;
                }
            }
        } 
        #endregion

        private void CheckResponse(HttpResponseMessage response, string responseBody)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw JsonConvert.DeserializeObject<VsoException>(responseBody);
            }
            else if (response.StatusCode == HttpStatusCode.NonAuthoritativeInformation)
            {
                throw new VsoException(HttpStatusCode.NonAuthoritativeInformation.ToString());
            }
        }

        private HttpClient GetHttpClient(string mediaType = JSON_MEDIA_TYPE)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            _authProvider.ProcessHeaders(client.DefaultRequestHeaders);
            return client;
        }

        private string ConstructUrl(string projectName, string path)
        {
            return ConstructUrl(projectName, path, new Dictionary<string, object>());
        }

        protected virtual string ConstructUrl(string projectName, string path, IDictionary<string, object> arguments)
        {
            if (!arguments.ContainsKey("api-version"))
            {
                arguments.Add("api-version", ApiVersion);
            }

            StringBuilder resultUrl = new StringBuilder(
                string.IsNullOrEmpty(projectName) ? 
                string.Format("{0}/_apis/{1}", _rootUrl, SubSystemName) :
                string.Format("{0}/{1}/_apis/{2}", _rootUrl, projectName, SubSystemName));

            if(!string.IsNullOrEmpty(path))
            {
                resultUrl.AppendFormat("/{0}", path);
            }

            resultUrl.AppendFormat("?{0}", string.Join("&", arguments.Where(kvp => kvp.Value != null).Select(kvp => 
                {
                    if (kvp.Value is IEnumerable<string>)
                    {
                        return string.Join("&", ((IEnumerable<string>)kvp.Value).Select(v => string.Format("{0}={1}", kvp.Key, v)));
                    }
                    else
                    {
                        return string.Format("{0}={1}", kvp.Key, kvp.Value);
                    }
                }
                )));
            return resultUrl.ToString();
        }
    }

    public abstract class RestClientPreview1 : VsoRestClient
    {
        protected override string ApiVersion
        {
            get { return VsoAPI.Preview1; }
        }

        public RestClientPreview1(string rootUrl, IHttpRequestHeaderFilter authProvider) : base(rootUrl, authProvider) { }
    }

    public abstract class RestClientPreview2 : VsoRestClient
    {
        protected override string ApiVersion
        {
            get { return VsoAPI.Preview2; }
        }

        public RestClientPreview2(string rootUrl, IHttpRequestHeaderFilter authProvider) : base(rootUrl, authProvider) { }
    }

    public abstract class RestClientVersion1 : VsoRestClient
    {
        protected override string ApiVersion
        {
            get { return VsoAPI.Version1; }
        }

        public RestClientVersion1(string rootUrl, IHttpRequestHeaderFilter authProvider) : base(rootUrl, authProvider) { }
    }

    /// <summary>
    /// HttpClient extensions
    /// </summary>
    public static class HttpClientExtensions
    {
        public async static Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri, HttpContent content)
        {
            var method = new HttpMethod("PATCH");

            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = content
            };

            return await client.SendAsync(request);
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class VsoException : Exception
    {
        public VsoException()
        { }

        public VsoException(string errorMessage) : base(errorMessage)
        { }

        [JsonProperty(PropertyName = "$id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "innerException")]
        public object ServerInnerException { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string ErrorMessage { get; set; }

        public override string Message { get { return ErrorMessage; } }

        [JsonProperty(PropertyName = "typeName")]
        public string TypeName { get; set; }

        [JsonProperty(PropertyName = "typeKey")]
        public string TypeKey { get; set; }

        [JsonProperty(PropertyName = "errorCode")]
        public int ErrorCode { get; set; }

        [JsonProperty(PropertyName = "eventId")]
        public int EventId { get; set; }
    }
}
