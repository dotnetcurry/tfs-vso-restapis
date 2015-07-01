using Microsoft.VisualStudio.TestTools.UnitTesting;
using VisualStudioOnline.Api.Rest.Test.Properties;
using VisualStudioOnline.Api.Rest.V1.Client;

namespace VisualStudioOnline.Api.Rest.Test.V1
{
    [TestClass]
    public class TagRestClientTest : VsoTestBase
    {
        private IVsoTag _client;

        protected override void OnInitialize(VsoClient vsoClient)
        {
            _client = vsoClient.GetService<IVsoTag>();
        }

        [TestMethod]
        public void TestGetTagList()
        {
            var tags = _client.GetTagList(Settings.Default.ProjectId).Result;
        }

        [TestMethod]
        public void TestCreateAndUpdateTag()
        {
            var newTag = _client.CreateTag(Settings.Default.ProjectId, "TestTag").Result;
            
            newTag.Name = "TestTagRenamed";
            newTag = _client.UpdateTag(Settings.Default.ProjectId, newTag).Result;

            newTag = _client.GetTag(Settings.Default.ProjectId, "TestTagRenamed").Result;

            var response = _client.DeleteTag(Settings.Default.ProjectId, newTag).Result;
        }
    }
}
