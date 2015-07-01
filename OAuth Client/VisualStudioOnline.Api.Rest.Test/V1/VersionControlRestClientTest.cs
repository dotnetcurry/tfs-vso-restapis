using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using VisualStudioOnline.Api.Rest.V1.Client;

namespace VisualStudioOnline.Api.Rest.Test.V1
{
    [TestClass]
    public class VersionControlRestClientTest : VsoTestBase
    {
        private IVsoVersionControl _client;

        protected override void OnInitialize(VsoClient vsoClient)
        {
            _client = vsoClient.GetService<IVsoVersionControl>();
        }

        [TestMethod]
        public void TestGetBranches()
        {
            var rootBranches = _client.GetRootBranches().Result;
            var branch = _client.GetBranch(rootBranches[1].Path).Result;
        }

        [TestMethod]
        public void TestGetLabels()
        {
            var labels = _client.GetLabels().Result;
            var label = _client.GetLabel(labels[0].Id.ToString()).Result;
            var items = _client.GetLabelledItems(label.Id.ToString()).Result;
        }

        [TestMethod]
        public void TestGetShelvesets()
        {
            var shelvesets = _client.GetShelvesets().Result;
            var shelveset = _client.GetShelveset(shelvesets[0].Id, true, true).Result;

            var changes = _client.GetShelvesetChanges(shelveset.Id).Result;
            var workItems = _client.GetShelvesetWorkItems(shelveset.Id).Result;
        }

        [TestMethod]
        public void TestGetChangesets()
        {
            var changesets = _client.GetChangesets().Result;
            var changesetBatch = _client.GetChangesets(new int[] { changesets[0].Id, changesets[1].Id }).Result;

            var changeset = _client.GetChangeset(changesets[0].Id).Result;
            var change = _client.GetChangesetChanges(changeset.Id).Result;
            var workitems = _client.GetChangesetWorkItems(changeset.Id).Result;
        }

        [TestMethod]
        public void TestGetVersionControlItems()
        {
            var changesets = _client.GetChangesets().Result;

            if (changesets.Count > 1)
            {
                var change = _client.GetChangesetChanges(changesets[1].Id).Result;

                var vcItemContent = _client.GetVersionControlItemContent(new VersionSearchFilter() { Path = change[0].Item.Path }).Result;
                var vcItem = _client.GetVersionControlItem(new VersionSearchFilter() { Path = change[0].Item.Path }).Result;

                var vcVersions = _client.GetVersionControlItemVersions(new VersionSearchFilter() { Path = change[0].Item.Path }).Result;

                if (change.Count > 1)
                {
                    var multipleVcVersions = _client.GetVersionControlItemVersions(
                        new List<VersionSearchFilter>()
                {
                    new VersionSearchFilter() { Path = change[0].Item.Path, Type = VersionType.Tip },
                    new VersionSearchFilter() { Path = change[1].Item.Path }
                }).Result;
                }
            }
        }
    }
}
