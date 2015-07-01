using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VisualStudioOnline.Api.Rest.V1.Model;

namespace VisualStudioOnline.Api.Rest.V1.Client
{
    public interface IVsoVersionControl
    {
        Task<JsonCollection<Branch>> GetRootBranches(bool? includeChildren = null, bool? includeDeleted = null);

        Task<Branch> GetBranch(string path, bool? includeChildren = null, bool? includeParent = null, bool? includeDeleted = null);

        Task<JsonCollection<Label>> GetLabels(string name = null, string owner = null, string itemLabelFilter = null, int? top = null, int? skip = null);

        Task<Label> GetLabel(string labelId, int? maxItemCount = null);

        Task<JsonCollection<VersionControlItem>> GetLabelledItems(string labelId, int? top = null, int? skip = null);

        Task<JsonCollection<Shelveset>> GetShelvesets(string owner = null, string maxCommentLength = null, int? top = null, int? skip = null);

        Task<Shelveset> GetShelveset(string shelvesetId, bool? includeDetails = null, bool? includeWorkItems = null, int? maxChangeCount = null, string maxCommentLength = null);

        Task<JsonCollection<VersionControlItemChange>> GetShelvesetChanges(string shelvesetId, int? top = null, int? skip = null);

        Task<JsonCollection<WorkItemInfo>> GetShelvesetWorkItems(string shelvesetId);

        Task<JsonCollection<Changeset>> GetChangesets(ChangesetSearchFilter? filter = null, int? top = null, int? skip = null, OrderBy? order = null, int? maxCommentLength = null);

        Task<JsonCollection<Changeset>> GetChangesets(int[] ids, int? maxCommentLength = null);

        Task<Changeset> GetChangeset(int changesetId, bool? includeDetails = null, bool? includeWorkItems = null, int? maxChangeCount = null, int? maxCommentLength = null);

        Task<JsonCollection<VersionControlItemChange>> GetChangesetChanges(int changesetId, int? top = null, int? skip = null);

        Task<JsonCollection<WorkItemInfo>> GetChangesetWorkItems(int changesetId);

        Task<string> GetVersionControlItemContent(VersionSearchFilter filter);

        Task<VersionControlItemVersion> GetVersionControlItem(VersionSearchFilter filter);

        Task<JsonCollection<VersionControlItemVersion>> GetVersionControlItemVersions(VersionSearchFilter filter);

        Task<JsonCollection<VersionControlItemVersion>> GetVersionControlItemVersions(IList<VersionSearchFilter> filters);
    }

    public enum VersionType
    {
        Branch,
        Changeset,
        Shelveset,
        Change,
        Date,
        MergeSource,
        Latest,
        Tip
    }

    public enum VersionOptions
    {
        None,
        Previous,
        UseRename
    }

    public struct VersionSearchFilter
    {
        [JsonProperty(PropertyName = "versionType", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public VersionType? Type;

        [JsonProperty(PropertyName = "versionOptions", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public VersionOptions? Options;

        [JsonProperty(PropertyName = "version")]
        public string Value;

        [JsonProperty(PropertyName = "path")]
        public string Path;

        [JsonProperty(PropertyName = "recursionLevel", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public RecursionLevel? Recursion;
    }

    public enum RecursionLevel
    {
        OneLevel,
        Full
    }

    public enum OrderBy
    {
        asc,
        desc
    }

    public struct ChangesetSearchFilter
    {
        public string ItemPath;
        public string Version;
        public VersionType? VersionType;
        public VersionOptions? VersionOption;
        public string Author;

        public int? FromId;
        public int? ToId;

        public DateTime? FromDate;
        public DateTime? ToDate;
    }

    /// <summary>
    /// TFS Version Control REST API client
    /// </summary>
    public class VersionControlRestClient : RestClientVersion1, IVsoVersionControl
    {
        protected override string SubSystemName
        {
            get
            {
                return "tfvc";
            }
        }

        public VersionControlRestClient(string url, NetworkCredential userCredential)
            : base(url, new BasicAuthenticationFilter(userCredential))
        {
        }


        /// <summary>
        /// Get a list of root branches
        /// </summary>
        /// <param name="includeChildren"></param>
        /// <param name="includeParent"></param>
        /// <param name="includeDeleted"></param>
        /// <returns></returns>
        public async Task<JsonCollection<Branch>> GetRootBranches(bool? includeChildren = null, bool? includeDeleted = null)
        {
            string response = await GetResponse("branches", new Dictionary<string, object>() { { "includeChildren", includeChildren }, { "includeDeleted", includeDeleted } });
            return JsonConvert.DeserializeObject<JsonCollection<Branch>>(response);
        }

        /// <summary>
        /// Get a branch
        /// </summary>
        /// <param name="path"></param>
        /// <param name="includeChildren"></param>
        /// <param name="includeParent"></param>
        /// <param name="includeDeleted"></param>
        /// <returns></returns>
        public async Task<Branch> GetBranch(string path, bool? includeChildren = null, bool? includeParent = null, bool? includeDeleted = null)
        {
            string response = await GetResponse(string.Format("branches/{0}", path), 
                new Dictionary<string, object>() { { "includeChildren", includeChildren }, { "includeParent", includeParent }, { "includeDeleted", includeDeleted } });
            return JsonConvert.DeserializeObject<Branch>(response);
        }

        /// <summary>
        /// Get list of labels
        /// </summary>
        /// <param name="name"></param>
        /// <param name="owner"></param>
        /// <param name="itemLabelFilter"></param>
        /// <param name="top"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public async Task<JsonCollection<Label>> GetLabels(string name = null, string owner = null, string itemLabelFilter = null, int? top = null, int? skip = null)
        {
            string response = await GetResponse("labels",
                new Dictionary<string, object>() { { "name", name }, { "owner", owner }, { "itemlabelFilter", itemLabelFilter }, { "$top", top }, { "$skip", skip } });
            return JsonConvert.DeserializeObject<JsonCollection<Label>>(response);
        }

        /// <summary>
        /// Get label by id
        /// </summary>
        /// <param name="labelId"></param>
        /// <param name="maxItemCount"></param>
        /// <returns></returns>
        public async Task<Label> GetLabel(string labelId, int? maxItemCount = null)
        {
            string response = await GetResponse(string.Format("labels/{0}", labelId), new Dictionary<string, object>() { { "maxItemCount", maxItemCount } });
            return JsonConvert.DeserializeObject<Label>(response);
        }

        /// <summary>
        /// Get list of labelled items
        /// </summary>
        /// <param name="labelId"></param>
        /// <param name="top"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public async Task<JsonCollection<VersionControlItem>> GetLabelledItems(string labelId, int? top = null, int? skip = null)
        {
            string response = await GetResponse(string.Format("labels/{0}/items", labelId), new Dictionary<string, object>() { { "$top", top }, { "$skip", skip } });
            return JsonConvert.DeserializeObject<JsonCollection<VersionControlItem>>(response);
        }


        /// <summary>
        /// Get list of shelvesets
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="maxCommentLength"></param>
        /// <param name="top"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public async Task<JsonCollection<Shelveset>> GetShelvesets(string owner = null, string maxCommentLength = null, int? top = null, int? skip = null)
        {
            string response = await GetResponse("shelvesets",
                new Dictionary<string, object>() { { "owner", owner }, { "maxCommentLength", maxCommentLength }, { "$top", top }, { "$skip", skip } });
            return JsonConvert.DeserializeObject<JsonCollection<Shelveset>>(response);
        }

        /// <summary>
        /// Get a shelveset by id
        /// </summary>
        /// <param name="shelvesetId"></param>
        /// <param name="includeDetails"></param>
        /// <param name="includeWorkItems"></param>
        /// <param name="maxChangeCount"></param>
        /// <param name="maxCommentLength"></param>
        /// <returns></returns>
        public async Task<Shelveset> GetShelveset(string shelvesetId, bool? includeDetails = null, bool? includeWorkItems = null, int? maxChangeCount = null, string maxCommentLength = null)
        {
            string response = await GetResponse(string.Format("shelvesets/{0}", shelvesetId),
                new Dictionary<string, object>() { { "includeDetails", includeDetails }, { "includeWorkItems", includeWorkItems }, { "maxChangeCount", maxChangeCount }, { "maxCommentLength", maxCommentLength } });
            return JsonConvert.DeserializeObject<Shelveset>(response);
        }

        /// <summary>
        /// Get shelveset changes
        /// </summary>
        /// <param name="shelvesetId"></param>
        /// <param name="top"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public async Task<JsonCollection<VersionControlItemChange>> GetShelvesetChanges(string shelvesetId, int? top = null, int? skip = null)
        {
            string response = await GetResponse(string.Format("shelvesets/{0}/changes", shelvesetId),
                new Dictionary<string, object>() { { "$top", top }, { "$skip", skip } });
            return JsonConvert.DeserializeObject<JsonCollection<VersionControlItemChange>>(response);
        }

        /// <summary>
        /// Get shelveset work items
        /// </summary>
        /// <param name="shelvesetId"></param>
        /// <returns></returns>
        public async Task<JsonCollection<WorkItemInfo>> GetShelvesetWorkItems(string shelvesetId)
        {
            string response = await GetResponse(string.Format("shelvesets/{0}/workitems", shelvesetId));
            return JsonConvert.DeserializeObject<JsonCollection<WorkItemInfo>>(response);
        }

        /// <summary>
        /// Get list of changesets
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="top"></param>
        /// <param name="skip"></param>
        /// <param name="order"></param>
        /// <param name="maxCommentLength"></param>
        /// <returns></returns>
        public async Task<JsonCollection<Changeset>> GetChangesets(ChangesetSearchFilter? filter = null, int? top = null, int? skip = null, OrderBy? order = null, int? maxCommentLength = null)
        {
            var arguments = new Dictionary<string, object>() { { "maxCommentLength", maxCommentLength }, { "$top", top }, { "$skip", skip } };
            if(filter.HasValue) 
            {
                arguments.Add("searchCriteria.itemPath", filter.Value.ItemPath);
                arguments.Add("searchCriteria.version", filter.Value.Version);
                arguments.Add("searchCriteria.versionType", filter.Value.VersionType.ToString());
                arguments.Add("searchCriteria.versionOption", filter.Value.VersionOption.ToString());
                arguments.Add("searchCriteria.author", filter.Value.Author);

                arguments.Add("searchCriteria.fromId", filter.Value.FromId);
                arguments.Add("searchCriteria.toId", filter.Value.ToId);

                arguments.Add("searchCriteria.fromDate", filter.Value.FromDate);
                arguments.Add("searchCriteria.toDate", filter.Value.ToDate);
            }
            if(order.HasValue) { arguments.Add("$orderby", order.Value == OrderBy.asc ? "id asc" : "id desc"); }

            string response = await GetResponse("changesets", arguments);
            return JsonConvert.DeserializeObject<JsonCollection<Changeset>>(response);
        }

        /// <summary>
        /// Get list of changesets by a list of IDs
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="maxCommentLength"></param>
        /// <returns></returns>
        public async Task<JsonCollection<Changeset>> GetChangesets(int[] ids, int? maxCommentLength = null)
        {
            string response = await PostResponse("changesetsBatch", new { changesetIds = ids, commentLength = maxCommentLength });
            return JsonConvert.DeserializeObject<JsonCollection<Changeset>>(response);
        }

        /// <summary>
        /// Get a changeset
        /// </summary>
        /// <param name="changesetId"></param>
        /// <param name="includeDetails"></param>
        /// <param name="includeWorkItems"></param>
        /// <param name="maxChangeCount"></param>
        /// <param name="maxCommentLength"></param>
        /// <returns></returns>
        public async Task<Changeset> GetChangeset(int changesetId, bool? includeDetails = null, bool? includeWorkItems = null, int? maxChangeCount = null, int? maxCommentLength = null)
        {
            string response = await GetResponse(string.Format("changesets/{0}", changesetId), 
                new Dictionary<string, object>() 
                { 
                    { "includeDetails", includeDetails }, 
                    { "includeWorkItems", includeWorkItems }, 
                    { "maxChangeCount", maxChangeCount }, 
                    { "maxCommentLength", maxCommentLength } 
                });
            return JsonConvert.DeserializeObject<Changeset>(response);
        }

        /// <summary>
        /// Get list of changes in a changeset
        /// </summary>
        /// <param name="changesetId"></param>
        /// <param name="top"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public async Task<JsonCollection<VersionControlItemChange>> GetChangesetChanges(int changesetId, int? top = null, int? skip = null)
        {
            string response = await GetResponse(string.Format("changesets/{0}/changes", changesetId),
                new Dictionary<string, object>() { { "$top", top }, { "$skip", skip } });
            return JsonConvert.DeserializeObject<JsonCollection<VersionControlItemChange>>(response);
        }

        /// <summary>
        /// Get list of associated work items
        /// </summary>
        /// <param name="changesetId"></param>
        /// <returns></returns>
        public async Task<JsonCollection<WorkItemInfo>> GetChangesetWorkItems(int changesetId)
        {
            string response = await GetResponse(string.Format("changesets/{0}/workitems", changesetId));
            return JsonConvert.DeserializeObject<JsonCollection<WorkItemInfo>>(response);
        }
       
        /// <summary>
        /// Get file content
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<string> GetVersionControlItemContent(VersionSearchFilter filter)
        {
            string response = await GetResponse(string.Format("items/{0}", filter.Path), AddVersionCriteria(filter, new Dictionary<string, object>()), null, HTML_MEDIA_TYPE);
            return response;
        }

        /// <summary>
        /// Get version control item metadata 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<VersionControlItemVersion> GetVersionControlItem(VersionSearchFilter filter)
        {
            string response = await GetResponse(string.Format("items/{0}", filter.Path), AddVersionCriteria(filter, new Dictionary<string, object>()));
            return JsonConvert.DeserializeObject<VersionControlItemVersion>(response);
        }

        /// <summary>
        /// Get version control item metadata 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<JsonCollection<VersionControlItemVersion>> GetVersionControlItemVersions(VersionSearchFilter filter)
        {
            var arguments = new Dictionary<string, object>() { { "scopepath", filter.Path } };

            string response = await GetResponse("items", AddVersionCriteria(filter, arguments));
            return JsonConvert.DeserializeObject<JsonCollection<VersionControlItemVersion>>(response);
        }

        /// <summary>
        /// Get metadata for several version control items 
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public async Task<JsonCollection<VersionControlItemVersion>> GetVersionControlItemVersions(IList<VersionSearchFilter> filters)
        {
            string response = await PostResponse("itemBatch", new { itemDescriptors = filters });
            return JsonConvert.DeserializeObject<JsonCollection<VersionControlItemVersion>>(response);
        }

        private Dictionary<string, object> AddVersionCriteria(VersionSearchFilter filter, Dictionary<string, object> arguments)
        {
            if (filter.Type.HasValue) 
            { 
                arguments.Add("versionType", filter.Type.ToString());

                if (filter.Type.Value != VersionType.Latest && filter.Type.Value != VersionType.Tip)
                {
                    arguments.Add("version", filter.Value);
                }
            }
            
            if (filter.Options.HasValue) { arguments.Add("versionOptions", filter.Options.ToString()); }
            if (filter.Recursion.HasValue) { arguments.Add("recursionLevel", filter.Recursion.ToString()); }

            return arguments;
        }
    }
}
