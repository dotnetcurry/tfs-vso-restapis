using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using VisualStudioOnline.Api.Rest.V1.Model;

namespace VisualStudioOnline.Api.Rest.V1.Client
{
    public interface IVsoGit
    {
        Task<JsonCollection<Repository>> GetRepositories();

        Task<Repository> GetRepository(string id);

        Task<Repository> CreateRepository(string name, string projectId);

        Task<Repository> RenameRepository(string id, string newName);

        Task<string> DeleteRepository(string id);

        Task<JsonCollection<GitReference>> GetRefs(string repoId, string filter = null);

        Task<JsonCollection<GitBranchInfo>> GetBranchStatistics(string repoId);

        Task<GitBranchInfo> GetBranchStatistics(string repoId, string branchName, BaseVersionType? type = null, string baseVersion = null);
    }

    public enum BaseVersionType 
    { 
        Branch, 
        Tag, 
        Commit 
    }

    public class GitRestClient : RestClientVersion1, IVsoGit
    {
        protected override string SubSystemName
        {
            get { return "git"; }
        }

        public GitRestClient(string url, NetworkCredential userCredential)
            : base(url, new BasicAuthenticationFilter(userCredential))
        {
        }

        /// <summary>
        /// Get a list of repositories
        /// </summary>
        /// <returns></returns>
        public async Task<JsonCollection<Repository>> GetRepositories()
        {
            string response = await GetResponse("repositories");
            return JsonConvert.DeserializeObject<JsonCollection<Repository>>(response);
        }

        /// <summary>
        /// Get a repository
        /// </summary>
        /// <param name="repoId"></param>
        /// <returns></returns>
        public async Task<Repository> GetRepository(string repoId)
        {
            string response = await GetResponse(string.Format("repositories/{0}", repoId));
            return JsonConvert.DeserializeObject<Repository>(response);
        }

        /// <summary>
        /// Create a repository
        /// </summary>
        /// <param name="name"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<Repository> CreateRepository(string name, string projectId)
        {
            string response = await PostResponse("repositories", new { name = name, project = new { id = projectId } });
            return JsonConvert.DeserializeObject<Repository>(response);
        }

        /// <summary>
        /// Rename a repository
        /// </summary>
        /// <param name="repoId"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public async Task<Repository> RenameRepository(string repoId, string newName)
        {
            string response = await PatchResponse(string.Format("repositories/{0}", repoId), new { name = newName }, null, JSON_MEDIA_TYPE);
            return JsonConvert.DeserializeObject<Repository>(response);
        }

        /// <summary>
        /// Delete a repository
        /// </summary>
        /// <param name="repoId"></param>
        /// <returns></returns>
        public async Task<string> DeleteRepository(string repoId)
        {
            string response = await DeleteResponse(string.Format("repositories/{0}", repoId));
            return response;
        }

        /// <summary>
        /// Get a list of references
        /// </summary>
        /// <param name="repoId"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<JsonCollection<GitReference>> GetRefs(string repoId, string filter = null)
        {
            string response = await GetResponse(string.IsNullOrEmpty(filter) ? 
                string.Format("repositories/{0}/refs", repoId) : string.Format("repositories/{0}/refs/{1}", repoId, filter));
            return JsonConvert.DeserializeObject<JsonCollection<GitReference>>(response);
        }

        /// <summary>
        /// Get branch statistics
        /// </summary>
        /// <param name="repoId"></param>
        /// <returns></returns>
        public async Task<JsonCollection<GitBranchInfo>> GetBranchStatistics(string repoId)
        {
            string response = await GetResponse(string.Format("repositories/{0}/stats/branches", repoId));
            return JsonConvert.DeserializeObject<JsonCollection<GitBranchInfo>>(response);
        }

        /// <summary>
        /// A version of a branch
        /// </summary>
        /// <param name="repoId"></param>
        /// <param name="branchName"></param>
        /// <param name="type"></param>
        /// <param name="baseVersion"></param>
        /// <returns></returns>
        public async Task<GitBranchInfo> GetBranchStatistics(string repoId, string branchName, BaseVersionType? type = null, string baseVersion = null)
        {
            string response = await GetResponse(string.Format("repositories/{0}/stats/branches/{1}", repoId, branchName),
                 new Dictionary<string, object>() { 
                    { "baseVersionType", type != null ? type.Value.ToString() :  null},
                    { "baseVersion", baseVersion}});
            return JsonConvert.DeserializeObject<GitBranchInfo>(response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repoId"></param>
        /// <param name="objectId"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public async Task<string> GetTreeMetadata(string repoId, string objectId, bool? recursive = null)
        {
            string response = await GetResponse(string.Format("repositories/{0}/trees/{1}", repoId, objectId));
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repoId"></param>
        /// <param name="objectId"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<string> DownloadTree(string repoId, string objectId, string fileName = null)
        {
            string response = await GetResponse(string.Format("repositories/{0}/trees/{1}", repoId, objectId), 
                new Dictionary<string, object>() { { "fileName", fileName}, { "$format", "zip"} });
            return response;
        }
    }
}
