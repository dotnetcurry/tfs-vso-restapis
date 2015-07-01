using Microsoft.VisualStudio.TestTools.UnitTesting;
using VisualStudioOnline.Api.Rest.Test.Properties;
using VisualStudioOnline.Api.Rest.V1.Client;

namespace VisualStudioOnline.Api.Rest.Test.V1
{
    [TestClass]
    public class AccountRestClientTest
    {
        private IVsoAccount _client;

        [TestInitialize]
        public void Initialize()
        {
            _client = new AccountRestClient(Settings.Default.AccessToken);
        }

        [TestMethod]
        public void TestGetAccountList()
        {
            var accounts = _client.GetAccountList().Result;
        }

        [TestMethod]
        public void TestGetAccount()
        {
            var account = _client.GetAccount(Settings.Default.AccountName).Result;
        }
    }
}
