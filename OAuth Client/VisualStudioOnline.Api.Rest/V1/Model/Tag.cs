using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;

namespace VisualStudioOnline.Api.Rest.V1.Model
{
    [DebuggerDisplay("{Name}")]
    public class Tag : ObjectWithId<string>
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "active")]
        public bool Active { get; set; }
    }
}
