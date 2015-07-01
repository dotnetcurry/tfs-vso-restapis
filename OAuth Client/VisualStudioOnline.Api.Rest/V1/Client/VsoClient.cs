using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace VisualStudioOnline.Api.Rest.V1.Client
{
    /// <summary>
    /// Main entry point for using VSO REST APIs
    /// </summary>
    public class VsoClient
    {
        private const string ACCOUNT_ROOT_URL = "https://{0}.visualstudio.com/{1}";
        private const string DEFAULT_COLLECTION = "DefaultCollection";

        private string _rootUrl;
        private NetworkCredential _userCredential;

        private static Dictionary<Type, Type> _serviceMapping = new Dictionary<Type, Type>()
        {
            { typeof(IVsoGit), typeof(GitRestClient) },
            { typeof(IVsoBuild), typeof(BuildRestClient) },
            { typeof(IVsoProjectCollection), typeof(ProjectCollectionRestClient) },
            { typeof(IVsoProject), typeof(ProjectRestClient) },
            { typeof(IVsoTag), typeof(TagRestClient) },
            { typeof(IVsoVersionControl), typeof(VersionControlRestClient) },
            { typeof(IVsoWit), typeof(WitRestClient) },
            { typeof(IVsoSimple), typeof(SimpleRestClient) }
        };

        public VsoClient(string accountName, NetworkCredential userCredential, string collectionName = DEFAULT_COLLECTION)
        {
            _rootUrl = string.Format(ACCOUNT_ROOT_URL, accountName, collectionName);
            _userCredential = userCredential;
        }

        public T GetService<T>()
        {
            if(!_serviceMapping.ContainsKey(typeof(T)))
            {
                throw new VsoException("Unknown service requested.");
            }

            return (T)Activator.CreateInstance(_serviceMapping[typeof(T)], _rootUrl, _userCredential);
        }

        public Task<T> Get<T>(string url)
        {
            return GetService<IVsoSimple>().Get<T>(url);
        }
    }

    public interface IVsoSimple
    {
        Task<T> Get<T>(string url);
    }

    public class SimpleRestClient : RestClientVersion1, IVsoSimple
    {
        public SimpleRestClient(string rootUrl, NetworkCredential userCredential) : base(rootUrl, new BasicAuthenticationFilter(userCredential)) { }

        protected override string SubSystemName
        {
            get { return string.Empty; }
        }

        protected override string ConstructUrl(string projectName, string path, IDictionary<string, object> arguments)
        {
            return path;
        }

        /// <summary>
        /// Get object of type T from the URL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<T> Get<T>(string url)
        {
            string response = await GetResponse(url);
            return JsonConvert.DeserializeObject<T>(response);
        }
    }
}
