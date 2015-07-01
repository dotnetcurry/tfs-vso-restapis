using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using VisualStudioOnline.Api.Rest.Test.Properties;
using VisualStudioOnline.Api.Rest.V1.Client;

namespace VisualStudioOnline.Api.Rest.Test.V1
{
    public class VsoTestBase
    {
        protected VsoClient _vsoClient;

        [TestInitialize]
        public void Initialize()
        {
            _vsoClient = new VsoClient(Settings.Default.AccountName, new NetworkCredential(Settings.Default.UserName, Settings.Default.Password));
            OnInitialize(_vsoClient);
        }

        protected virtual void OnInitialize(VsoClient vsoClient)
        {
        }
    }
}
