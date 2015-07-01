using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using VisualStudioOnline.Api.Rest.V1.Model;

namespace VisualStudioOnline.Api.Rest.V1.Client
{
    public interface IVsoAccount
    {
        Task<JsonCollection<Account>> GetAccountList();

        Task<Account> GetAccount(string name);
    }

    /// <summary>
    /// Account REST API client
    /// </summary>
    public class AccountRestClient : RestClientVersion1, IVsoAccount
    {
        private const string VSSPS_ROOT_URL = "https://app.vssps.visualstudio.com";
        
        protected override string SubSystemName
        {
            get 
            {
                return "accounts";
            }
        }

         public AccountRestClient(string authToken)
            : base(VSSPS_ROOT_URL, new OAuthFilter(authToken))
        { }

        /// <summary>
        /// Get account list for current user
        /// </summary>
        /// <returns></returns>
        public async Task<JsonCollection<Account>> GetAccountList()
        {
            string response = await GetResponse(string.Empty);
            return JsonConvert.DeserializeObject<JsonCollection<Account>>(response);            
        }

        /// <summary>
        /// Get specific account
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Account> GetAccount(string name)
        {
            string response = await GetResponse(string.Format("/{0}", name));
            return JsonConvert.DeserializeObject<Account>(response);
        }
    }
}
