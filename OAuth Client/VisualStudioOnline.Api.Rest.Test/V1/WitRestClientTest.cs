using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using VisualStudioOnline.Api.Rest.Test.Properties;
using VisualStudioOnline.Api.Rest.V1.Client;
using VisualStudioOnline.Api.Rest.V1.Model;

namespace VisualStudioOnline.Api.Rest.Test.V1
{
    [TestClass]
    public class WitRestClientTest : VsoTestBase
    {
        private IVsoWit _client;

        protected override void OnInitialize(VsoClient vsoClient)
        {
            _client = vsoClient.GetService<IVsoWit>();
        }

        [TestMethod]
        public void TestGetRelationTypes()
        {
            var relations = _client.GetWorkItemRelationTypes().Result;
            var relation = _client.GetWorkItemRelationType(relations.Items[0].ReferenceName).Result;
        }

        [TestMethod]
        public void TestGetFields()
        {
            var fields = _client.GetFields().Result;
            var field = _client.GetField(fields.Items[0].ReferenceName).Result;
        }

        [TestMethod]
        public void TestGetWorkItemTypeCategories()
        {
            var workItemTypeCategories = _client.GetWorkItemTypeCategories(Settings.Default.ProjectName).Result;
            var workItemTypeCategory = _client.GetWorkItemTypeCategory(Settings.Default.ProjectName, workItemTypeCategories.Items[0].ReferenceName).Result;
        }

        [TestMethod]
        public void TestGetWorkItemTypes()
        {
            var workItemTypes = _client.GetWorkItemTypes(Settings.Default.ProjectName).Result;
            var workItemType = _client.GetWorkItemType(Settings.Default.ProjectName, workItemTypes.Items[0].Name).Result;
        }

        [TestMethod]
        public void TestCRUDForClassificationNodes()
        {
            var nodes = _client.GetClassificationNodes(Settings.Default.ProjectName).Result;

            var rootArea = _client.GetAreaNode(Settings.Default.ProjectName, 5).Result;
            var rootIteration = _client.GetIterationNode(Settings.Default.ProjectName, 5).Result;

            var newArea = _client.CreateAreaNode(Settings.Default.ProjectName, "Test Area").Result;
            var newIteration = _client.CreateIterationNode(Settings.Default.ProjectName, "Test Iteration").Result;
            
            newArea.Name = "Renamed";
            newArea = _client.UpdateNode(Settings.Default.ProjectName, "Test Area", newArea).Result;

            newIteration.Attributes = new NodeAttributes() { StartDate = DateTime.Now.AddDays(-1), FinishDate = DateTime.Now };
            newIteration = _client.UpdateNode(Settings.Default.ProjectName, "Test Iteration", newIteration).Result;

            var iteration1 = _client.GetIterationNode(Settings.Default.ProjectName, "Iteration 1").Result;
            var area1 = _client.GetAreaNode(Settings.Default.ProjectName, "Area 1").Result;

            newArea = _client.MoveNode(Settings.Default.ProjectName, "Area 1", newArea).Result;
            newIteration = _client.MoveNode(Settings.Default.ProjectName, "Iteration 1", newIteration).Result;

            var result = _client.DeleteAreaNode(Settings.Default.ProjectName, string.Format("Area 1/{0}", newArea.Name), area1).Result;
            result = _client.DeleteIterationNode(Settings.Default.ProjectName, string.Format("Iteration 1/{0}", newIteration.Name), iteration1).Result;
        }

        [TestMethod]
        public void TestGetWorkItemHistory()
        {
            var history = _client.GetWorkItemHistory(Settings.Default.WorkItemId).Result;
            var revHistory = _client.GetWorkItemRevisionHistory(Settings.Default.WorkItemId, Settings.Default.WorkItemRevision).Result;
        }

        [TestMethod]
        public void TestGetWorkItemRevisions()
        {
            var revisions = _client.GetWorkItemRevisions(Settings.Default.WorkItemId, null, null, RevisionExpandOptions.all).Result;
            var revision = _client.GetWorkItemRevision(Settings.Default.WorkItemId, Settings.Default.WorkItemRevision).Result;

            var areaPath = revision.Fields["System.AreaPath"];
        }

        [TestMethod]
        public void TestGetWorkItemUpdates()
        {
            var updates = _client.GetWorkItemUpdates(Settings.Default.WorkItemId).Result;
            var update = _client.GetWorkItemUpdate(Settings.Default.WorkItemId, Settings.Default.WorkItemRevision).Result;
        }

        [TestMethod]
        public void TestUploadDownloadAttachments()
        {
            var fileRef = _client.UploadAttachment("Test.txt", "Hello world").Result;
            string content = _client.DownloadAttachment(fileRef.Id).Result;

            var bug = _client.GetWorkItem(Settings.Default.WorkItemId, RevisionExpandOptions.relations).Result;

            // Add attachment to WI
            bug.Relations.Add(new WorkItemRelation()
            {
                Url = fileRef.Url,
                Rel = "AttachedFile",
                Attributes = new RelationAttributes() { Comment = "Hello world" }
            });

            bug = _client.UpdateWorkItem(bug).Result;

            // Remove attachment from WI
            bug.Relations.RemoveAt(2);
            bug = _client.UpdateWorkItem(bug).Result;            
        }

        [TestMethod]
        public void TestCreateAndUpdateWorkItem()
        {
            var defaultValues = _client.GetWorkItemTypeDefaultValues(Settings.Default.ProjectName, "Bug").Result;
            var workItems = _client.GetWorkItems(new int[] { Settings.Default.WorkItemId }, RevisionExpandOptions.all).Result;

            // Create new work item
            var bug = new WorkItem();
            bug.Fields["System.Title"] = "Test bug 1";
            bug.Fields["System.History"] = DateTime.Now.ToString();
            bug = _client.CreateWorkItem(Settings.Default.ProjectName, "Bug", bug).Result;

            var other = workItems[0];

            // Update fields, add a link
            bug.Fields["System.Title"] = bug.Fields["System.Title"] + " (updated)";
            bug.Fields["System.Tags"] = "SimpleTag";
            
            bug.Relations.Add(new WorkItemRelation()
                {
                    Url = other.Url,
                    Rel = "System.LinkTypes.Dependency-Forward",
                    Attributes = new RelationAttributes() { Comment = "Hello world" }
                });

            bug = _client.UpdateWorkItem(bug).Result;

            //TODO: update link
            //bug.Relations[0].Attributes.IsLocked = true;
            //bug = _client.UpdateWorkItem(bug).Result;

            // Remove link
            bug.Relations.RemoveAt(0);
            bug = _client.UpdateWorkItem(bug).Result;
            
            // Add hyperlink
            bug.Relations.Add(new WorkItemRelation()
            {
                Url = "http://www.bing.com",
                Rel = "Hyperlink",
                Attributes = new RelationAttributes() { Comment = "Hello world" }
            });

            bug = _client.UpdateWorkItem(bug).Result;

            // Remove it
            bug.Relations.RemoveAt(0);
            bug = _client.UpdateWorkItem(bug).Result;

            // Get all revisions
            var revisions = _vsoClient.Get<JsonCollection<WorkItem>>(bug.References.Revisions.Href).Result;            
        }

        [TestMethod]
        public void TestCreateAndUpdateQueries()
        {
            var rootQueries = _client.GetQueries(Settings.Default.ProjectName).Result;
            var sharedQueries = _client.GetQuery(Settings.Default.ProjectName, "Shared Queries", 2, QueryExpandOptions.all).Result;

            var newQuery = _client.CreateQuery(Settings.Default.ProjectName, 
                "Shared Queries/Troubleshooting", 
                string.Format("REST {0}", DateTime.Now.Ticks),
                "select System.Id from Issue").Result;

            newQuery.Name = newQuery.Name + "_Renamed";
            newQuery.Wiql = "select System.Id, System.AssignedTo from Issue";
            newQuery = _client.UpdateQuery(Settings.Default.ProjectName, newQuery).Result;
            
            string response = _client.DeleteQuery(Settings.Default.ProjectName, newQuery).Result;
            newQuery = _client.GetQuery(Settings.Default.ProjectName, newQuery.Id, null, QueryExpandOptions.all, true).Result;
            newQuery = _client.UndeleteQuery(Settings.Default.ProjectName, newQuery).Result;
            response = _client.DeleteQuery(Settings.Default.ProjectName, newQuery).Result;

            var newFolder = _client.CreateQueryFolder(Settings.Default.ProjectName, "Shared Queries", string.Format("REST {0}", DateTime.Now.Ticks)).Result;

            newFolder = _client.MoveQuery(Settings.Default.ProjectName, "Shared Queries/Troubleshooting", newFolder).Result;

            response = _client.DeleteQuery(Settings.Default.ProjectName, newFolder).Result;
        }

        [TestMethod]
        public void TestRunQueries()
        {
            const string FLAT_QUERY = "select System.Id, System.AssignedTo from Issue";
            var flatResult = _client.RunFlatQuery(Settings.Default.ProjectName, FLAT_QUERY).Result;

            // Run one hop query
            var linkResult = _client.RunLinkQuery(Settings.Default.ProjectName, "select System.Id from WorkItemLinks where ([Source].[System.WorkItemType] <> '' and [Source].[System.State] <> '') and ([System.Links.LinkType] <> '') and ([Target].[System.WorkItemType]<> '') order by System.Id mode(MayContain)").Result;

            // Run tree query
            var treeResult = _client.RunLinkQuery(Settings.Default.ProjectName, "select System.Id from WorkItemLinks where ([Source].[System.WorkItemType] <> '' and [Source].[System.State] <> '') and ([System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward') and ([Target].[System.WorkItemType] <> '') order by System.Id mode(Recursive)").Result;

            var newQuery = _client.CreateQuery(Settings.Default.ProjectName,
                "Shared Queries/Troubleshooting",
                string.Format("REST {0}", DateTime.Now.Ticks),
                FLAT_QUERY).Result;

            var result = _client.RunFlatQuery(Settings.Default.ProjectName, newQuery).Result;

            _client.DeleteQuery(Settings.Default.ProjectName, newQuery).Wait();
        }
    }
}
