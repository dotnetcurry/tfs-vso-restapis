using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace VisualStudioOnline.Api.Rest.V1.Model
{
    [DebuggerDisplay("{Name}")]
    public class ProjectTeam : ObjectWithId<Guid>
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "identityUrl")]
        public string IdentityUrl { get; set; }
    }

    [DebuggerDisplay("{DisplayName}")]
    public class UserIdentity : ObjectWithId<Guid>
    {
        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }

        [JsonProperty(PropertyName = "uniqueName")]
        public string UniqueName { get; set; }

        [JsonProperty(PropertyName = "imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty(PropertyName = "isContainer")]
        public bool? IsContainer { get; set; }
    }
}
