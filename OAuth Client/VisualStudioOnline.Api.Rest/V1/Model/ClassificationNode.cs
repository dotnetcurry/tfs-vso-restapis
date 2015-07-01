using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace VisualStudioOnline.Api.Rest.V1.Model
{
    public enum NodeType
    {
        area,
        iteration,
    }

    public class NodeAttributes
    {
        [JsonProperty(PropertyName = "startDate")]
        public DateTime StartDate { get; set; }

        [JsonProperty(PropertyName = "finishDate")]
        public DateTime FinishDate { get; set; }
    }

    [DebuggerDisplay("{Name}")]
    public class ClassificationNode : ObjectWithId<int, SelfLink>
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "structureType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public NodeType StructureType { get; set; }

        [JsonProperty(PropertyName = "hasChildren")]
        public bool HasChildren { get; set; }

        [JsonProperty(PropertyName = "children")]
        public List<ClassificationNode> Children { get; set; }

        [JsonProperty(PropertyName = "attributes")]
        public NodeAttributes Attributes { get; set; }
    }
}
