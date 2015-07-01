using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using VisualStudioOnline.Api.Rest.V1.Model;

namespace VisualStudioOnline.Api.Rest.V1.Client
{
    public interface IVsoProjectCollection
    {
        Task<JsonCollection<TeamProjectCollection>> GetProjectCollections(int? top = null, int? skip = null);

        Task<TeamProjectCollection> GetProjectCollection(string projectCollectionName);
    }

    public class ProjectCollectionRestClient : RestClientVersion1, IVsoProjectCollection
    {
        protected override string SubSystemName
        {
            get { return "projectcollections"; }
        }

        public ProjectCollectionRestClient(string url, NetworkCredential userCredential)
            : base(url, new BasicAuthenticationFilter(userCredential))
        {
        }

        /// <summary>
        /// Get team project collection list
        /// </summary>
        /// <param name="stateFilter"></param>
        /// <param name="top"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public async Task<JsonCollection<TeamProjectCollection>> GetProjectCollections(int? top = null, int? skip = null)
        {
            string response = await GetResponse(string.Empty,
                new Dictionary<string, object>() { { "$top", top }, { "$skip", skip } });
            return JsonConvert.DeserializeObject<JsonCollection<TeamProjectCollection>>(response);
        }

        /// <summary>
        /// Get team project collection by name
        /// </summary>
        /// <param name="projectNameOrId"></param>
        /// <param name="includecapabilities"></param>
        /// <returns></returns>
        public async Task<TeamProjectCollection> GetProjectCollection(string projectCollectionName)
        {
            string response = await GetResponse(projectCollectionName);
            return JsonConvert.DeserializeObject<TeamProjectCollection>(response);
        }
     }

}
