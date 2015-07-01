using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using VisualStudioOnline.Api.Rest.V1.Model;

namespace VisualStudioOnline.Api.Rest.V1.Client
{
    public interface IVsoWit
    {
        #region WorkItem Type
        Task<WorkItemTypeDefaults> GetWorkItemTypeDefaultValues(string projectName, string workItemTypeName);
        
        Task<JsonCollection<WorkItemType>> GetWorkItemTypes(string projectName);

        Task<WorkItemType> GetWorkItemType(string projectName, string typeName);
        #endregion

        #region WorkItem
        Task<WorkItem> CreateWorkItem(string projectName, string workItemTypeName, WorkItem workItem);

        Task<WorkItem> UpdateWorkItem(WorkItem workItem);

        Task<WorkItem> GetWorkItem(int workItemId, RevisionExpandOptions options = RevisionExpandOptions.none);

        Task<JsonCollection<WorkItem>> GetWorkItems(int[] workItemIds, RevisionExpandOptions options = RevisionExpandOptions.none, DateTime? asOfDate = null, string[] fields = null);

        Task<JsonCollection<HistoryComment>> GetWorkItemHistory(int workitemId, int? top = null, int? skip = null);

        Task<HistoryComment> GetWorkItemRevisionHistory(int workitemId, int revision);

        Task<JsonCollection<WorkItemRelationType>> GetWorkItemRelationTypes();

        Task<WorkItemRelationType> GetWorkItemRelationType(string relationName);

        Task<JsonCollection<Field>> GetFields();

        Task<Field> GetField(string fieldName);
        #endregion

        #region WorkItem Type Category
        Task<JsonCollection<WorkItemTypeCategory>> GetWorkItemTypeCategories(string projectName);

        Task<WorkItemTypeCategory> GetWorkItemTypeCategory(string projectName, string categoryName);
        #endregion

        #region Classification Nodes
        Task<JsonCollection<ClassificationNode>> GetClassificationNodes(string projectName, int? depth = null);

        Task<ClassificationNode> GetAreaNode(string projectName, int? depth = null);

        Task<ClassificationNode> GetAreaNode(string projectName, string nodePath, int? depth = null);

        Task<ClassificationNode> GetIterationNode(string projectName, int? depth = null);

        Task<ClassificationNode> GetIterationNode(string projectName, string nodePath, int? depth = null);

        Task<ClassificationNode> CreateAreaNode(string projectName, string nodeName, string rootPath = null);

        Task<ClassificationNode> CreateIterationNode(string projectName, string nodeName, string rootPath = null, DateTime? startDate = null, DateTime? endDate = null);

        Task<ClassificationNode> MoveNode(string projectName, string newPath, ClassificationNode node);

        Task<ClassificationNode> UpdateNode(string projectName, string path, ClassificationNode node);

        Task<string> DeleteAreaNode(string projectName, string areaPath, ClassificationNode reclassificationNode);

        Task<string> DeleteIterationNode(string projectName, string iterationPath, ClassificationNode reclassificationNode);
        #endregion

        #region WorkItem Revisions
        Task<JsonCollection<WorkItem>> GetWorkItemRevisions(int workItemId, int? top = null, int? skip = null, RevisionExpandOptions options = RevisionExpandOptions.none);

        Task<WorkItem> GetWorkItemRevision(int workItemId, int revision, RevisionExpandOptions options = RevisionExpandOptions.none);

        Task<JsonCollection<WorkItemUpdate>> GetWorkItemUpdates(int workItemId, int? top = null, int? skip = null);

        Task<WorkItemUpdate> GetWorkItemUpdate(int workItemId, int revisionId);
        #endregion

        #region Attachments
        Task<string> DownloadAttachment(string attachmentId);

        Task<ObjectWithId<string>> UploadAttachment(string projectName, string area, string fileName, byte[] content);

        Task<ObjectWithId<string>> UploadAttachment(string fileName, string content);
        #endregion

        #region Queries
        Task<JsonCollection<Query>> GetQueries(string projectName, string folderPath = null, int? depth = null, QueryExpandOptions options = QueryExpandOptions.none, bool? includeDeleted = null);

        Task<Query> GetQuery(string projectName, string folderPathOrId, int? depth = null, QueryExpandOptions options = QueryExpandOptions.none, bool? includeDeleted = null);

        Task<Query> CreateQuery(string projectName, string parentPath, string queryName, string queryText);

        Task<Query> CreateQueryFolder(string projectName, string parentPath, string folderName);

        Task<Query> UpdateQuery(string projectName, Query query);

        Task<Query> MoveQuery(string projectName, string newPath, Query query);

        Task<string> DeleteQuery(string projectName, Query query);

        Task<string> DeleteQuery(string projectName, string queryPath);

        Task<Query> UndeleteQuery(string projectName, Query query, bool? undeleteDescendants = null);

        Task<FlatQueryResult> RunFlatQuery(string projectName, string queryText);

        Task<LinkQueryResult> RunLinkQuery(string projectName, string queryText);

        Task<FlatQueryResult> RunFlatQuery(string projectName, Query query);

        Task<LinkQueryResult> RunLinkQuery(string projectName, Query query);
        #endregion
    }

    public enum QueryExpandOptions
    {
        none,
        all,
        wiql,
    }

    public enum RevisionExpandOptions
    {
        all,
        relations,
        none
    }

    /// <summary>
    /// WIT REST API client v.1.0
    /// </summary>
    public class WitRestClient : RestClientVersion1, IVsoWit
    {
        protected override string SubSystemName
        {
            get { return "wit"; }
        }

        public WitRestClient(string url, NetworkCredential userCredential)
            : base(url, new BasicAuthenticationFilter(userCredential))
        {
        }

        /// <summary>
        /// Get the default values that will be filled in automatically when you create a new work item of a specific type.
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="workItemTypeName"></param>
        /// <returns></returns>
        public async Task<WorkItemTypeDefaults> GetWorkItemTypeDefaultValues(string projectName, string workItemTypeName)
        {
            string response = await GetResponse(string.Format("workitems/${0}", workItemTypeName), projectName);
            return JsonConvert.DeserializeObject<WorkItemTypeDefaults>(response);
        }

       
        /// <summary>
        /// Create new work item
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="workItemTypeName"></param>
        /// <param name="workItem"></param>
        /// <returns></returns>
        public async Task<WorkItem> CreateWorkItem(string projectName, string workItemTypeName, WorkItem workItem)
        {
            List<object> updateList = new List<object>();

            updateList.AddRange(workItem.FieldUpdates);
            updateList.AddRange(workItem.RelationUpdates);

            string response = await PatchResponse(string.Format("workitems/${0}", workItemTypeName), updateList, projectName);
            JsonConvert.PopulateObject(response, workItem);

            PrepareWorkItem(workItem);

            return workItem;
        }

        /// <summary>
        /// TODO: not working now
        /// </summary>
        /// <param name="workItem"></param>
        /// <returns></returns>
        public async Task<WorkItem> UpdateWorkItem(WorkItem workItem)
        {
            List<Update> updateList = new List<Update>();

            updateList.AddRange(workItem.FieldUpdates);
            updateList.AddRange(workItem.RelationUpdates);

            string response = await PatchResponse(string.Format("workitems/{0}", workItem.Id), updateList);

            workItem.Relations.Clear();            

            JsonConvert.PopulateObject(response, workItem);

            PrepareWorkItem(workItem);

            return workItem;
        }

        /// <summary>
        /// Get work item by id
        /// </summary>
        /// <param name="workItemId"></param>
        /// <param name="includeLinks"></param>
        /// <returns></returns>
        public async Task<WorkItem> GetWorkItem(int workItemId, RevisionExpandOptions options = RevisionExpandOptions.none)
        {
            string response = await GetResponse(string.Format("workitems/{0}", workItemId), new Dictionary<string, object>() { { "$expand", options } });
            var workItem = JsonConvert.DeserializeObject<WorkItem>(response);

            PrepareWorkItem(workItem);

            return workItem;
        }

        /// <summary>
        /// Get a list of work items by ids
        /// </summary>
        /// <param name="workItemIds"></param>
        /// <param name="includeLinks"></param>
        /// <param name="asOfDate"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public async Task<JsonCollection<WorkItem>> GetWorkItems(int[] workItemIds, RevisionExpandOptions options = RevisionExpandOptions.none, DateTime? asOfDate = null, string[] fields = null)
        {
            var arguments = new Dictionary<string, object>() { { "ids", string.Join(",", workItemIds) }, { "$expand", options.ToString() } };
            if (asOfDate.HasValue) { arguments.Add("asof", asOfDate.Value.ToUniversalTime().ToString("u")); }
            if (fields != null) { arguments.Add("fields", string.Join(",", fields)); }

            string response = await GetResponse("workitems", arguments);
            var workItems = JsonConvert.DeserializeObject<JsonCollection<WorkItem>>(response);

            for (int i = 0; i < workItems.Count; i++)
            {
                PrepareWorkItem(workItems[i]);
            }

            return workItems;
        }

        /// <summary>
        /// Get work item history comments
        /// </summary>
        /// <param name="workitemId"></param>
        /// <param name="top"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public async Task<JsonCollection<HistoryComment>> GetWorkItemHistory(int workitemId, int? top = null, int? skip = null)
        {
            string response = await GetResponse(string.Format("workitems/{0}/history", workitemId),
                new Dictionary<string, object>() { { "$top", top }, {"$skip", skip } });
            return JsonConvert.DeserializeObject<JsonCollection<HistoryComment>>(response);
        }

        /// <summary>
        /// Get work item revision history comment
        /// </summary>
        /// <param name="workitemId"></param>
        /// <param name="revision"></param>
        /// <returns></returns>
        public async Task<HistoryComment> GetWorkItemRevisionHistory(int workitemId, int revision)
        {
            string response = await GetResponse(string.Format("workitems/{0}/history/{1}", workitemId, revision));
            return JsonConvert.DeserializeObject<HistoryComment>(response);
        }

        /// <summary>
        /// Get work item relation types
        /// </summary>
        /// <returns></returns>
        public async Task<JsonCollection<WorkItemRelationType>> GetWorkItemRelationTypes()
        {
            string response = await GetResponse("workitemrelationtypes");
            return JsonConvert.DeserializeObject<JsonCollection<WorkItemRelationType>>(response);
        }

        /// <summary>
        /// Get specific work item relation type
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public async Task<WorkItemRelationType> GetWorkItemRelationType(string relationName)
        {
            string response = await GetResponse(string.Format("workitemrelationtypes/{0}", relationName));
            return JsonConvert.DeserializeObject<WorkItemRelationType>(response);
        }

        /// <summary>
        /// Get collection fields
        /// </summary>
        /// <returns></returns>
        public async Task<JsonCollection<Field>> GetFields()
        {
            string response = await GetResponse("fields");
            return JsonConvert.DeserializeObject<JsonCollection<Field>>(response);
        }

        /// <summary>
        /// Get specific field
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public async Task<Field> GetField(string fieldName)
        {
            string response = await GetResponse(string.Format("fields/{0}", fieldName));
            return JsonConvert.DeserializeObject<Field>(response);
        }

        /// <summary>
        /// Get work item type categories for a project
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public async Task<JsonCollection<WorkItemTypeCategory>> GetWorkItemTypeCategories(string projectName)
        {
            string response = await GetResponse("workitemtypecategories", projectName);
            return JsonConvert.DeserializeObject<JsonCollection<WorkItemTypeCategory>>(response);
        }

        /// <summary>
        /// Get work item type category for a project
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public async Task<WorkItemTypeCategory> GetWorkItemTypeCategory(string projectName, string categoryName)
        {
            string response = await GetResponse(string.Format("workitemtypecategories/{0}", categoryName), projectName);
            return JsonConvert.DeserializeObject<WorkItemTypeCategory>(response);
        }

        /// <summary>
        /// Get work item types for the project
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public async Task<JsonCollection<WorkItemType>> GetWorkItemTypes(string projectName)
        {
            string response = await GetResponse("workitemtypes", projectName);
            return JsonConvert.DeserializeObject<JsonCollection<WorkItemType>>(response);
        }

        /// <summary>
        /// Get specific work item type for the project
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public async Task<WorkItemType> GetWorkItemType(string projectName, string typeName)
        {
            string response = await GetResponse(string.Format("workitemtypes/{0}", typeName), projectName);
            return JsonConvert.DeserializeObject<WorkItemType>(response);
        }


        /// <summary>
        /// Get root classification nodes (areas and iterations)
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public async Task<JsonCollection<ClassificationNode>> GetClassificationNodes(string projectName, int? depth = null)
        {
            string response = await GetCssNode(projectName, string.Empty, depth);
            return JsonConvert.DeserializeObject<JsonCollection<ClassificationNode>>(response);
        }

        /// <summary>
        /// Get root area node
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public async Task<ClassificationNode> GetAreaNode(string projectName, int? depth = null)
        {
            return await GetAreaNode(projectName, string.Empty, depth);
        }

        /// <summary>
        /// Get area node by path
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="nodePath"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public async Task<ClassificationNode> GetAreaNode(string projectName, string nodePath, int? depth = null)
        {
            string path = "areas";
            if (!string.IsNullOrEmpty(nodePath)) { path = string.Format("{0}/{1}", path, nodePath); }

            string response = await GetCssNode(projectName, path, depth);
            return JsonConvert.DeserializeObject<ClassificationNode>(response);
        }

        /// <summary>
        /// Get root iteration node
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public async Task<ClassificationNode> GetIterationNode(string projectName, int? depth = null)
        {
            return await GetIterationNode(projectName, string.Empty, depth);
        }

        /// <summary>
        /// Get iteration path by path
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="nodePath"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public async Task<ClassificationNode> GetIterationNode(string projectName, string nodePath, int? depth = null)
        {
            string path = "iterations";
            if (!string.IsNullOrEmpty(nodePath)) { path = string.Format("{0}/{1}", path, nodePath); }

            string response = await GetCssNode(projectName, path, depth);
            return JsonConvert.DeserializeObject<ClassificationNode>(response);
        }

        /// <summary>
        /// Create a classification node
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="rootPath"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public async Task<ClassificationNode> CreateAreaNode(string projectName, string nodeName, string rootPath = null)
        {
            string response = await PostResponse(string.Format("classificationnodes/areas/{0}", rootPath), new { name = nodeName }, projectName);
            return JsonConvert.DeserializeObject<ClassificationNode>(response);
        }

        /// <summary>
        /// Create a classification node
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="rootPath"></param>
        /// <param name="nodeName"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<ClassificationNode> CreateIterationNode(string projectName, string nodeName, string rootPath = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            string response = await PostResponse(string.Format("classificationnodes/iterations/{0}", rootPath), new { name = nodeName, attributes = new { startDate = startDate, endDate = endDate } }, projectName);
            return JsonConvert.DeserializeObject<ClassificationNode>(response);
        }

        /// <summary>
        /// Move a classification node
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="newPath"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public async Task<ClassificationNode> MoveNode(string projectName, string newPath, ClassificationNode node)
        {
            string response = await PostResponse(string.Format("classificationnodes/{0}/{1}", node.StructureType == NodeType.area ? "areas" : "iterations", newPath), 
                new Dictionary<string, object>(), new { Id = node.Id }, projectName);
            JsonConvert.PopulateObject(response, node);
            return node;
        }

        /// <summary>
        /// Update a classification node
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="path"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public async Task<ClassificationNode> UpdateNode(string projectName, string path, ClassificationNode node)
        {
            string response = await PatchResponse(string.Format("classificationnodes/{0}/{1}", node.StructureType == NodeType.area ? "areas" : "iterations", path),
                new { name = node.Name, attributes = node.Attributes }, projectName, JSON_MEDIA_TYPE);
            JsonConvert.PopulateObject(response, node);
            return node;
        }

        /// <summary>
        /// Delete classification node
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="areaPath"></param>
        /// <param name="reclassificationNode"></param>
        /// <returns></returns>
        public async Task<string> DeleteAreaNode(string projectName, string areaPath, ClassificationNode reclassificationNode)
        {
            return await DeleteResponse(string.Format("classificationnodes/areas/{0}", areaPath), new Dictionary<string, object>() { { "$reclassifyId", reclassificationNode.Id } }, projectName);
        }

        /// <summary>
        /// Delete classification node
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="iterationPath"></param>
        /// <param name="reclassificationNode"></param>
        /// <returns></returns>
        public async Task<string> DeleteIterationNode(string projectName, string iterationPath, ClassificationNode reclassificationNode)
        {
            return await DeleteResponse(string.Format("classificationnodes/iterations/{0}", iterationPath), new Dictionary<string, object>() { { "$reclassifyId", reclassificationNode.Id } }, projectName);
        }

        /// <summary>
        /// Get work item revisions
        /// </summary>
        /// <param name="workItemId"></param>
        /// <param name="top"></param>
        /// <param name="skip"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<JsonCollection<WorkItem>> GetWorkItemRevisions(int workItemId, int? top = null, int? skip = null, RevisionExpandOptions options = RevisionExpandOptions.none)
        {
            string response = await GetResponse(string.Format("workitems/{0}/revisions", workItemId),
                new Dictionary<string, object>() { { "$top", top }, { "$skip", skip }, { "$expand", options } });
            var revisions = JsonConvert.DeserializeObject<JsonCollection<WorkItem>>(response);

            foreach (var revision in revisions.Items)
            {
                PrepareWorkItem(revision);
            }

            return revisions;
        }

        /// <summary>
        /// Get work item revision
        /// </summary>
        /// <param name="workItemId"></param>
        /// <param name="revision"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<WorkItem> GetWorkItemRevision(int workItemId, int revision, RevisionExpandOptions options = RevisionExpandOptions.none)
        {
            string response = await GetResponse(string.Format("workitems/{0}/revisions/{1}", workItemId, revision), new Dictionary<string, object>() { { "$expand", options } });
            var workItem = JsonConvert.DeserializeObject<WorkItem>(response);

            PrepareWorkItem(workItem);

            return workItem;
        }

        /// <summary>
        /// Get work item updates
        /// </summary>
        /// <param name="workItemId"></param>
        /// <param name="top"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public async Task<JsonCollection<WorkItemUpdate>> GetWorkItemUpdates(int workItemId, int? top = null, int? skip = null)
        {
            string response = await GetResponse(string.Format("workitems/{0}/updates", workItemId), new Dictionary<string, object>() { { "$top", top }, { "$skip", skip } });
            return JsonConvert.DeserializeObject<JsonCollection<WorkItemUpdate>>(response);
        }

        /// <summary>
        /// Get specific work item update
        /// </summary>
        /// <param name="workItemId"></param>
        /// <param name="revisionId"></param>
        /// <returns></returns>
        public async Task<WorkItemUpdate> GetWorkItemUpdate(int workItemId, int revisionId)
        {
            string response = await GetResponse(string.Format("workitems/{0}/updates/{1}", workItemId, revisionId));
            return JsonConvert.DeserializeObject<WorkItemUpdate>(response);
        }

        /// <summary>
        /// Download work item attachment
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<string> DownloadAttachment(string attachmentId)
        {
            return await GetResponse(string.Format("attachments/{0}", attachmentId));
        }

        /// <summary>
        /// Upload binary attachment
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="area"></param>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<ObjectWithId<string>> UploadAttachment(string projectName, string area, string fileName, byte[] content)
        {
            return await UploadAttachment(fileName, Convert.ToBase64String(content));
        }

        /// <summary>
        /// Upload text attachment
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="area"></param>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<ObjectWithId<string>> UploadAttachment(string fileName, string content)
        {
            string response = await PostResponse("attachments", new Dictionary<string, object>() { { "fileName", fileName } }, content, null);
            return JsonConvert.DeserializeObject<ObjectWithId<string>>(response);
        }

      
        /// <summary>
        /// Get queries by folder path
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="folderPathOrId"></param>
        /// <param name="depth"></param>
        /// <param name="options"></param>
        /// <param name="includeDeleted"></param>
        /// <returns></returns>
        public async Task<JsonCollection<Query>> GetQueries(string projectName, string folderPath = null, int? depth = null, QueryExpandOptions options = QueryExpandOptions.none, bool? includeDeleted = null)
        {
            string response = await GetResponse(string.IsNullOrEmpty(folderPath) ? "queries" : string.Format("queries/{0}", folderPath),
                new Dictionary<string, object>() { { "$expand", options }, { "$depth", depth }, { "$includeDeleted", includeDeleted } }, 
                projectName);
            return JsonConvert.DeserializeObject<JsonCollection<Query>>(response);
        }

        /// <summary>
        /// Get query or query folder
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="folderPathOrId"></param>
        /// <param name="depth"></param>
        /// <param name="options"></param>
        /// <param name="includeDeleted"></param>
        /// <returns></returns>
        public async Task<Query> GetQuery(string projectName, string folderPathOrId, int? depth = null, QueryExpandOptions options = QueryExpandOptions.none, bool? includeDeleted = null)
        {
            string response = await GetResponse(string.Format("queries/{0}", folderPathOrId),
                new Dictionary<string, object>() { { "$expand", options }, { "$depth", depth }, { "$includeDeleted", includeDeleted } }, 
                projectName);
            return JsonConvert.DeserializeObject<Query>(response);
        }

        /// <summary>
        /// Create new query
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="parentPath"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<Query> CreateQuery(string projectName, string parentPath, string queryName, string queryText)
        {
            string response = await PostResponse(string.Format("queries/{0}", parentPath), new Dictionary<string, object>(), new { name = queryName, wiql = queryText }, projectName);
            return JsonConvert.DeserializeObject<Query>(response);
        }

        /// <summary>
        /// Create new query folder
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="parentPath"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public async Task<Query> CreateQueryFolder(string projectName, string parentPath, string folderName)
        {
            string response = await PostResponse(string.Format("queries/{0}", parentPath), new Dictionary<string, object>(), new { name = folderName, isFolder = true }, projectName);
            return JsonConvert.DeserializeObject<Query>(response);
        }

        /// <summary>
        /// Update the existing query / query folder
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<Query> UpdateQuery(string projectName, Query query)
        {
            string response = await PatchResponse(string.Format("queries/{0}", query.Path), new { wiql = query.Wiql, name = query.Name }, projectName, JSON_MEDIA_TYPE);
            JsonConvert.PopulateObject(response, query);
            return query;
        }

        /// <summary>
        /// Move existing query / query folder
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="newPath"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<Query> MoveQuery(string projectName, string newPath, Query query)
        {
            string response = await PostResponse(string.Format("queries/{0}", newPath), new Dictionary<string, object>(), new { Id = query.Id }, projectName);
            JsonConvert.PopulateObject(response, query);
            return query;
        }

        /// <summary>
        /// Delete existing query / query folder
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<string> DeleteQuery(string projectName, Query query)
        {
            return await DeleteResponse(string.Format("queries/{0}", query.Id), projectName);
        }

        /// <summary>
        /// Delete existing query / query folder
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="queryPath"></param>
        /// <returns></returns>
        public async Task<string> DeleteQuery(string projectName, string queryPath)
        {
            return await DeleteResponse(string.Format("queries/{0}", queryPath), projectName);
        }

        /// <summary>
        /// Undelete query / query folder
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="query"></param>
        /// <param name="undeleteDescendants"></param>
        /// <returns></returns>
        public async Task<Query> UndeleteQuery(string projectName, Query query, bool? undeleteDescendants = null)
        {
            string response = await PatchResponse(string.Format("queries/{0}", query.Id),
                new Dictionary<string, object>() { { "$undeletedescendants", undeleteDescendants } },
                new { isDeleted = false }, projectName, JSON_MEDIA_TYPE);
            JsonConvert.PopulateObject(response, query);
            return query;
        }

        /// <summary>
        /// Run flat query
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="queryText"></param>
        /// <returns></returns>
        public async Task<FlatQueryResult> RunFlatQuery(string projectName, string queryText)
        {
            string response = await PostResponse("wiql", new { query = queryText }, projectName);
            return JsonConvert.DeserializeObject<FlatQueryResult>(response);
        }

        /// <summary>
        /// Run link query
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="queryText"></param>
        /// <returns></returns>
        public async Task<LinkQueryResult> RunLinkQuery(string projectName, string queryText)
        {
            string response = await PostResponse("wiql", new { query = queryText }, projectName);
            return JsonConvert.DeserializeObject<LinkQueryResult>(response);
        }

        /// <summary>
        /// Run flat query
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<FlatQueryResult> RunFlatQuery(string projectName, Query query)
        {
            string response = await GetResponse(string.Format("wiql/{0}", query.Id), new Dictionary<string, object>(), projectName);
            return JsonConvert.DeserializeObject<FlatQueryResult>(response);
        }

        /// <summary>
        /// Run link query
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<LinkQueryResult> RunLinkQuery(string projectName, Query query)
        {
            string response = await GetResponse(string.Format("wiql/{0}", query.Id), new Dictionary<string, object>(), projectName);
            return JsonConvert.DeserializeObject<LinkQueryResult>(response);
        }

        #region Helper methods
        private async Task<string> GetCssNode(string projectName, string nodePath, int? depth = null)
        {
            string path = "classificationnodes";
            if (!string.IsNullOrEmpty(nodePath)) { path = string.Format("{0}/{1}", path, nodePath); }

            string response = await GetResponse(path, new Dictionary<string, object>() { { "$depth", depth } }, projectName);
            return response;
        }

        private void PrepareWorkItem(WorkItem workItem)
        {
            for (int i = 0; i < workItem.Relations.Count; i++)
            {
                workItem.Relations[i].Index = i;
                workItem.Relations[i].Source = workItem;
            }           

            workItem.FieldUpdates.Clear();
            workItem.RelationUpdates.Clear();
        }
        #endregion
    }
}
