using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VisualStudioOnline.Api.Rest.V1.Model;

namespace VisualStudioOnline.Api.Rest.V1.Client
{
    public interface IVsoBuild
    {
        Task<JsonCollection<BuildDefinition>> GetBuildDefinitions(string projectName);

        Task<BuildDefinition> GetBuildDefinition(string projectName, int definitionId);

        Task<JsonCollection<string>> GetBuildQualities(string projectName);

        Task<string> AddBuildQuality(string projectName, string quality);

        Task<string> DeleteBuildQuality(string projectName, string quality);

        Task<JsonCollection<BuildQueue>> GetBuildQueues();

        Task<BuildQueue> GetBuildQueue(int queueId);

        Task<JsonCollection<BuildRequest>> GetBuildRequests(string projectName, string requestedFor = null,
            int? definitionId = null, int? queueId = null, int? maxCompletedAge = null,
            BuildStatus? status = null, int? top = null, int? skip = null);

        Task<BuildRequest> RequestBuild(string projectName, int buildDefinitionId, BuildReason reason, BuildPriority priority, int? queueId = null);

        Task<string> UpdateBuildRequest(string projectName, int requestId, BuildStatus newStatus);

        Task<string> CancelBuildRequest(string projectName, int requestId);

        Task<JsonCollection<Build>> GetBuilds(string projectName, string requestedFor = null,
           int? definitionId = null, DateTime? minFinishTime = null, string quality = null, BuildStatus? status = null, int? top = null, int? skip = null);

        Task<Build> GetBuild(string projectName, int buildId, params BuildDetails[] details);

        Task<Build> UpdateBuild(string projectName, int buildId, BuildStatus? status = null, string quality = null, bool? retainIndefinitely = null);

        Task<string> DeleteBuild(string projectName, int buildId);
    }

    public enum BuildDetails
    {
        ActivityProperties,
        ActivityTracking,
        AgentScopeActivityTracking,
        AssociatedChangeset,
        AssociatedCommit,
        AssociatedWorkItem,
        BuildError,
        BuildMessage,
        BuildProject,
        BuildStep,
        BuildWarning,
        CheckInOutcome,
        CompilationSummary,
        ConfigurationSummary,
        CustomSummaryInformation,
        DeploymentInformation,
        ExternalLink,
        GetStatus,
        OpenedWorkItem
    }

    /// <summary>
    /// 
    /// </summary>
    public class BuildRestClient : RestClientVersion1, IVsoBuild
    {
        protected override string SubSystemName
        {
            get { return "build"; }
        }

        public BuildRestClient(string url, NetworkCredential userCredential)
            : base(url, new BasicAuthenticationFilter(userCredential))
        {
        }

        /// <summary>
        /// Get a list of build definitions
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public async Task<JsonCollection<BuildDefinition>> GetBuildDefinitions(string projectName)
        {
            string response = await GetResponse("definitions", projectName);
            return JsonConvert.DeserializeObject<JsonCollection<BuildDefinition>>(response);
        }

        /// <summary>
        /// Get a build definition
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="definitionId"></param>
        /// <returns></returns>
        public async Task<BuildDefinition> GetBuildDefinition(string projectName, int definitionId)
        {
            string response = await GetResponse(string.Format("definitions/{0}", definitionId), projectName);
            return JsonConvert.DeserializeObject<BuildDefinition>(response);
        }

        /// <summary>
        /// Get a list of qualities
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public async Task<JsonCollection<string>> GetBuildQualities(string projectName)
        {
            string response = await GetResponse("qualities", projectName);
            return JsonConvert.DeserializeObject<JsonCollection<string>>(response);
        }

        /// <summary>
        /// Add a quality
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="quality"></param>
        /// <returns></returns>
        public async Task<string> AddBuildQuality(string projectName, string quality)
        {
            string response = await PutResponse(string.Format("qualities/{0}", quality), content: null, projectName: projectName);
            return response;
        }

        /// <summary>
        /// Remove a quality
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="quality"></param>
        /// <returns></returns>
        public async Task<string> DeleteBuildQuality(string projectName, string quality)
        {
            string response = await DeleteResponse(string.Format("qualities/{0}", quality), projectName);
            return response;
        }

        /// <summary>
        /// Get a list of queues
        /// </summary>
        /// <returns></returns>
        public async Task<JsonCollection<BuildQueue>> GetBuildQueues()
        {
            string response = await GetResponse("queues");
            return JsonConvert.DeserializeObject<JsonCollection<BuildQueue>>(response);
        }

        /// <summary>
        /// Get a queue
        /// </summary>
        /// <returns></returns>
        public async Task<BuildQueue> GetBuildQueue(int queueId)
        {
            string response = await GetResponse(string.Format("queues/{0}", queueId));
            return JsonConvert.DeserializeObject<BuildQueue>(response);
        }


        public async Task<JsonCollection<BuildRequest>> GetBuildRequests(string projectName, string requestedFor = null,
            int? definitionId = null, int? queueId = null, int? maxCompletedAge = null, 
            BuildStatus? status = null, int? top = null, int? skip = null)
        {
            string response = await GetResponse("requests", 
                new Dictionary<string, object>() { 
                    { "requestedFor", requestedFor},
                    { "definitionId", definitionId},
                    { "queueId", queueId},
                    { "maxCompletedAge", maxCompletedAge},
                    { "status", status != null ? status.Value.ToString() : null },
                    { "$top", top }, 
                    { "$skip", skip } }, 
                projectName);
            return JsonConvert.DeserializeObject<JsonCollection<BuildRequest>>(response);
        }

        /// <summary>
        /// Request a build
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="buildDefinitionId"></param>
        /// <param name="reason"></param>
        /// <param name="priority"></param>
        /// <param name="queueId"></param>
        /// <returns></returns>
        public async Task<BuildRequest> RequestBuild(string projectName, int buildDefinitionId, BuildReason reason, BuildPriority priority, int? queueId = null)
        {
            string response = await PostResponse("requests",
                new { definition = new { id = buildDefinitionId }, reason = reason.ToString(), priority = priority.ToString(), queue = new { id = queueId } },
                projectName);
            return JsonConvert.DeserializeObject<BuildRequest>(response);
        }

        /// <summary>
        /// Update the status of a request
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="requestId"></param>
        /// <param name="newStatus"></param>
        /// <returns></returns>
        public async Task<string> UpdateBuildRequest(string projectName, int requestId, BuildStatus newStatus)
        {
            string response = await PatchResponse(string.Format("requests/{0}", requestId), new { status = newStatus.ToString() }, projectName, JSON_MEDIA_TYPE);
            return response;
        }

        /// <summary>
        /// Cancel a build request
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public async Task<string> CancelBuildRequest(string projectName, int requestId)
        {
            string response = await DeleteResponse(string.Format("requests/{0}", requestId), projectName);
            return response;
        }

        /// <summary>
        /// Get a list of builds
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="requestedFor"></param>
        /// <param name="definitionId"></param>
        /// <param name="minFinishTime"></param>
        /// <param name="quality"></param>
        /// <param name="status"></param>
        /// <param name="top"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public async Task<JsonCollection<Build>> GetBuilds(string projectName, string requestedFor = null,
           int? definitionId = null, DateTime? minFinishTime = null, string quality = null, BuildStatus? status = null, int? top = null, int? skip = null)
        {
            string response = await GetResponse("builds",
                new Dictionary<string, object>() { 
                    { "requestedFor", requestedFor },
                    { "definitionId", definitionId },
                    { "minFinishTime", minFinishTime },
                    { "quality", quality },
                    { "status", status != null ? status.Value.ToString() : null },
                    { "$top", top }, 
                    { "$skip", skip } },
                projectName);
            return JsonConvert.DeserializeObject<JsonCollection<Build>>(response);
        }

        /// <summary>
        /// Get build details
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="buildId"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public async Task<Build> GetBuild(string projectName, int buildId, params BuildDetails[] details)
        {            
            var arguments = new Dictionary<string, object>();

            if (details != null)
            {
                arguments.Add("types", details.Select(d => d.ToString()));
            }

            string response = await GetResponse(string.Format("builds/{0}", buildId), arguments, projectName);
            return JsonConvert.DeserializeObject<Build>(response);
        }

        /// <summary>
        /// Modify a build
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="buildId"></param>
        /// <param name="status"></param>
        /// <param name="quality"></param>
        /// <param name="retainIndefinitely"></param>
        /// <returns></returns>
        public async Task<Build> UpdateBuild(string projectName, int buildId, BuildStatus? status = null, string quality = null, bool? retainIndefinitely = null)
        {
            string response = await PatchResponse(string.Format("builds/{0}", buildId), 
                new { status = status, quality = quality, retainIndefinitely = retainIndefinitely }, 
                projectName, 
                JSON_MEDIA_TYPE);

            return JsonConvert.DeserializeObject<Build>(response);
        }

        /// <summary>
        /// Delete a build
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="buildId"></param>
        /// <returns></returns>
        public async Task<string> DeleteBuild(string projectName, int buildId)
        {
            string response = await DeleteResponse(string.Format("builds/{0}", buildId), projectName);
            return response;
        }
    }
}
