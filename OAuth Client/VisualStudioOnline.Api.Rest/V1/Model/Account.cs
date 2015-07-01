using Newtonsoft.Json;
using System.Collections.Generic;

namespace VisualStudioOnline.Api.Rest.V1.Model
{
    public class Properties
    {
    }

    public class Account
    {
        [JsonProperty(PropertyName = "accountId")]
        public string AccountId { get; set; }

        [JsonProperty(PropertyName = "accountUri")]
        public string AccountUri { get; set; }

        [JsonProperty(PropertyName = "accountName")]
        public string AccountName { get; set; }

        [JsonProperty(PropertyName = "organizationName")]
        public string OrganizationName { get; set; }

        [JsonProperty(PropertyName = "accountType")]
        public string AccountType { get; set; }

        [JsonProperty(PropertyName = "accountOwner")]
        public string AccountOwner { get; set; }

        [JsonProperty(PropertyName = "createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty(PropertyName = "createdDate")]
        public string CreatedDate { get; set; }

        [JsonProperty(PropertyName = "accountStatus")]
        public string AccountStatus { get; set; }

        [JsonProperty(PropertyName = "lastUpdatedBy")]
        public string LastUpdatedBy { get; set; }

        [JsonProperty(PropertyName = "properties")]
        public Properties Properties { get; set; }

        [JsonProperty(PropertyName = "lastUpdatedDate")]
        public string LastUpdatedDate { get; set; }
    }
}
