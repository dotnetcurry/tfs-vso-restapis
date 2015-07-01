using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace VisualStudioOnline.Api.Rest.V1.Model
{
    public enum QueryType
    {
        flat,
        tree,
        oneHop
    }

    public class QueryLink : ObjectLink
    {
        [JsonProperty(PropertyName = "html")]
        public ObjectLink Html { get; set; }
    }

    [DebuggerDisplay("{Field.ReferenceName}")]
    public class SortColumn
    {
        [JsonProperty(PropertyName = "field")]
        public Field Field { get; set; }

        [JsonProperty(PropertyName = "descending")]
        public bool Descending { get; set; }
    }

    [DebuggerDisplay("{Name}")]
    public class Query : ObjectWithId<string, QueryLink>
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        [JsonProperty(PropertyName = "queryType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public QueryType QueryType { get; set; }

        [JsonProperty(PropertyName = "columns")]
        public List<Field> Columns { get; set; }

        [JsonProperty(PropertyName = "sortColumns")]
        public List<SortColumn> SortColumns { get; set; }

        [JsonProperty(PropertyName = "wiql")]
        public string Wiql { get; set; }

        [JsonProperty(PropertyName = "isFolder")]
        public bool IsFolder { get; set; }

        [JsonProperty(PropertyName = "hasChildren")]
        public bool HasChildren { get; set; }

        [JsonProperty(PropertyName = "children")]
        public List<Query> Children { get; set; }

        [JsonProperty(PropertyName = "isPublic")]
        public bool IsPublic { get; set; }
    }

    [DebuggerDisplay("{QueryType}")]
    public abstract class QueryResult
    {
        [JsonProperty(PropertyName = "queryType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public QueryType QueryType { get; set; }

        [JsonProperty(PropertyName = "asOf")]
        public DateTime AsOf { get; set; }

        [JsonProperty(PropertyName = "columns")]
        public List<Field> Columns { get; set; }

        [JsonProperty(PropertyName = "sortColumns")]
        public List<SortColumn> SortColumns { get; set; }
    }

    public class FlatQueryResult : QueryResult
    {
        [JsonProperty(PropertyName = "workItems")]
        public List<WorkItem> WorkItems { get; set; }
    }

    public class LinkQueryResult : QueryResult
    {
        [JsonProperty(PropertyName = "workItemRelations")]
        public List<WorkItemRelation> Relations { get; set; }
    }
}
