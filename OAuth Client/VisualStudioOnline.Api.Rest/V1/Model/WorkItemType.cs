using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Diagnostics;

namespace VisualStudioOnline.Api.Rest.V1.Model
{
    public enum FieldType
    {
        boolean,
        dateTime,
        @double,
        history,
        html,
        integer,
        plainText,
        @string,
        treePath                        
    }

    [DebuggerDisplay("{ReferenceName}")]
    public class Field : BaseObject
    {
        [JsonProperty(PropertyName = "referenceName")]
        public string ReferenceName { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "type")]
        public FieldType Type { get; set; }

        [JsonProperty(PropertyName = "readOnly")]
        public bool ReadOnly { get; set; }
    }

    [DebuggerDisplay("{Field.ReferenceName}")]
    public class FieldInstance
    {
        [JsonProperty(PropertyName = "field")]
        public Field Field { get; set; }

        [JsonProperty(PropertyName = "helpText")]
        public string HelpText { get; set; }
    }

    [DebuggerDisplay("{Name}")]
    public class WorkItemType : BaseObject
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "xmlForm")]
        public string Form { get; set; }

        [JsonProperty(PropertyName = "fieldInstances")]
        public List<FieldInstance> Fields { get; set; }
    }

    [DebuggerDisplay("{Name}")]
    public class WorkItemTypeCategory : BaseObject
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "referenceName")]
        public string ReferenceName { get; set; }

        [JsonProperty(PropertyName = "defaultWorkItemType")]
        public WorkItemType DefaultWorkItemType { get; set; }

        [JsonProperty(PropertyName = "workItemTypes")]
        public List<WorkItemType> workItemTypes { get; set; }
    }

    public class TypeLink
    {
        [JsonProperty(PropertyName = "workItemType")]
        public ObjectLink WorkItemTypeReference { get; set; }

        [JsonProperty(PropertyName = "fields")]
        public ObjectLink FieldsReference { get; set; }
    }

    public class WorkItemTypeDefaults : BaseObject<TypeLink>
    {
        [JsonProperty(PropertyName = "fields")]
        public dynamic Fields { get; set; }
    }
}
