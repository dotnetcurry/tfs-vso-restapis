using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace VisualStudioOnline.Api.Rest
{
    public interface IHttpRequestHeaderFilter
    {
        HttpRequestHeaders ProcessHeaders(HttpRequestHeaders headers);
    }

    public class BasicAuthenticationFilter : IHttpRequestHeaderFilter
    {
        private string _authToken;

        public BasicAuthenticationFilter(NetworkCredential userCredential)
        {
            _authToken = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", userCredential.UserName, userCredential.Password)));
        }

        public HttpRequestHeaders ProcessHeaders(HttpRequestHeaders headers)
        {
            headers.Authorization = new AuthenticationHeaderValue("Basic", _authToken);
            return headers;
        }
    }

    public class OAuthFilter : IHttpRequestHeaderFilter
    {
        private string _authToken;

        public OAuthFilter(string authToken)
        {
            _authToken = authToken;
        }

        public HttpRequestHeaders ProcessHeaders(HttpRequestHeaders headers)
        {
            headers.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
            return headers;
        }
    }
}
