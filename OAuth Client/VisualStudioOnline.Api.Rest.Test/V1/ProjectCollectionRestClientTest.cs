using Microsoft.VisualStudio.TestTools.UnitTesting;
using VisualStudioOnline.Api.Rest.V1.Client;

namespace VisualStudioOnline.Api.Rest.Test.V1
{
    [TestClass]
    public class ProjectCollectionRestClientTest : VsoTestBase
    {
        private IVsoProjectCollection _client;

        protected override void OnInitialize(VsoClient vsoClient)
        {
            _client = vsoClient.GetService<IVsoProjectCollection>();
        }

        [TestMethod]
        public void TestGetProjectCollections()
        {
            var collections = _client.GetProjectCollections().Result;
            var collection = _client.GetProjectCollection(collections[0].Name).Result;
        }
    }
}
