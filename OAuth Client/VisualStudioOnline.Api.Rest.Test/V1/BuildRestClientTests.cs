using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using VisualStudioOnline.Api.Rest.Test.Properties;
using VisualStudioOnline.Api.Rest.V1.Client;
using VisualStudioOnline.Api.Rest.V1.Model;

namespace VisualStudioOnline.Api.Rest.Test.V1
{
    [TestClass]
    public class BuildRestClientTests : VsoTestBase
    {
        private IVsoBuild _client;

        protected override void OnInitialize(VsoClient vsoClient)
        {
            _client = vsoClient.GetService<IVsoBuild>();
        }

        [TestMethod]
        public void TestGetBuildDefinitions()
        {
            var definitions = _client.GetBuildDefinitions(Settings.Default.ProjectName).Result;
            var definition = _client.GetBuildDefinition(Settings.Default.ProjectName, definitions.Items[0].Id).Result;
        }

        [TestMethod]
        public void TestBuildQualities()
        {
            var qualities = _client.GetBuildQualities(Settings.Default.ProjectName).Result;

            var newQuality = DateTime.Now.Ticks.ToString();
            var result = _client.AddBuildQuality(Settings.Default.ProjectName, newQuality).Result;
            qualities = _client.GetBuildQualities(Settings.Default.ProjectName).Result;

            result = _client.DeleteBuildQuality(Settings.Default.ProjectName, newQuality).Result;
            qualities = _client.GetBuildQualities(Settings.Default.ProjectName).Result;
        }

        [TestMethod]
        public void TestGetBuildQueues()
        {
            var queues = _client.GetBuildQueues().Result;
            var queue = _client.GetBuildQueue(queues.Items[0].Id).Result;
        }

        [TestMethod]
        public void TestBuildRequests()
        {
            var requests = _client.GetBuildRequests(Settings.Default.ProjectName).Result;
            var definitions = _client.GetBuildDefinitions(Settings.Default.ProjectName).Result;
            var result = _client.UpdateBuildRequest(Settings.Default.ProjectName, requests.Items[0].Id, BuildStatus.Cancelled).Result;

            var request = _client.RequestBuild(Settings.Default.ProjectName, definitions.Items[0].Id, BuildReason.Manual, BuildPriority.Low).Result;
            result = _client.CancelBuildRequest(Settings.Default.ProjectName, request.Id).Result;
        }

        [TestMethod]
        public void TestBuilds()
        {
            var builds = _client.GetBuilds(Settings.Default.ProjectName).Result;

            if (builds.Items.Count > 0)
            {
                var build = _client.GetBuild(Settings.Default.ProjectName, builds.Items[0].Id, BuildDetails.BuildMessage, BuildDetails.GetStatus).Result;

                build = _client.UpdateBuild(Settings.Default.ProjectName, build.Id, BuildStatus.Cancelled).Result;
                var result = _client.DeleteBuild(Settings.Default.ProjectName, build.Id).Result;
                builds = _client.GetBuilds(Settings.Default.ProjectName).Result;
            }
        }
    }
}
