using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace VisualStudioOnline.Api.Rest.V1.Model
{
    [DebuggerDisplay("{Path}")]
    public class Branch : BaseObject
    {
        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "owner")]
        public UserIdentity Owner { get; set; }

        [JsonProperty(PropertyName = "createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty(PropertyName = "relatedBranches")]
        public List<Branch> RelatedBranches { get; set; }

        [JsonProperty(PropertyName = "children")]
        public List<Branch> Children { get; set; }

        [JsonProperty(PropertyName = "mappings")]
        public List<object> Mappings { get; set; }

        [JsonProperty(PropertyName = "isDeleted")]
        public bool IsDeleted { get; set; }
    }

    public class LabelLink : ObjectLink
    {
        [JsonProperty(PropertyName = "items")]
        public ObjectLink Items { get; set; }

        [JsonProperty(PropertyName = "owner")]
        public ObjectLink Owner { get; set; }
    }

    [DebuggerDisplay("{Name}")]
    public class Label : ObjectWithId<int, LabelLink>
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "labelScope")]
        public string LabelScope { get; set; }

        [JsonProperty(PropertyName = "modifiedDate")]
        public DateTime ModifiedDate { get; set; }

        [JsonProperty(PropertyName = "owner")]
        public UserIdentity Owner { get; set; }
    }

    [DebuggerDisplay("{FileName}")]
    public class ContentMetadata
    {
        [JsonProperty(PropertyName = "encoding")]
        public int Encoding { get; set; }

        [JsonProperty(PropertyName = "contentType")]
        public string ContentType { get; set; }

        [JsonProperty(PropertyName = "fileName")]
        public string FileName { get; set; }

        [JsonProperty(PropertyName = "extension")]
        public string Extension { get; set; }
    }

    [DebuggerDisplay("{Path}")]
    public class VersionControlItem : BaseObject
    {
        [JsonProperty(PropertyName = "version")]
        public int Version { get; set; }

        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        [JsonProperty(PropertyName = "isFolder")]
        public bool IsFolder { get; set; }

        [JsonProperty(PropertyName = "isBranch")]
        public bool IsBranch { get; set; }

        [JsonProperty(PropertyName = "contentMetadata")]
        public ContentMetadata contentMetadata { get; set; }

        [JsonProperty(PropertyName = "_links")]
        public SelfLink _links { get; set; }
    }

    [DebuggerDisplay("{Path}")]
    public class VersionControlItemVersion : VersionControlItem
    {
        [JsonProperty(PropertyName = "changeDate")]
        public DateTime ChangeDate { get; set; }
    }

    public class ShelvesetLink : ObjectLink
    {
        [JsonProperty(PropertyName = "changes")]
        public ObjectLink Changes { get; set; }

        [JsonProperty(PropertyName = "workItems")]
        public ObjectLink WorkItems { get; set; }

        [JsonProperty(PropertyName = "owner")]
        public ObjectLink Owner { get; set; }
    }

    public class PolicyOverride
    {
        [JsonProperty(PropertyName = "policyFailures")]
        public List<object> PolicyFailures { get; set; }
    }

    [DebuggerDisplay("{Item.Path}")]
    public class VersionControlItemChange
    {
        [JsonProperty(PropertyName = "item")]
        public VersionControlItem Item { get; set; }

        [JsonProperty(PropertyName = "changeType")]
        public string ChangeType { get; set; }
    }

    [DebuggerDisplay("{Id}")]
    public class WorkItemInfo
    {
        [JsonProperty(PropertyName = "webUrl")]
        public string WebUrl { get; set; }

        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "workItemType")]
        public string WorkItemType { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }
    }

    [DebuggerDisplay("{Name}")]
    public class Shelveset : ObjectWithId<string, ShelvesetLink>
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "owner")]
        public UserIdentity owner { get; set; }

        [JsonProperty(PropertyName = "createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty(PropertyName = "comment")]
        public string Comment { get; set; }

        [JsonProperty(PropertyName = "commentTruncated")]
        public bool CommentTruncated { get; set; }

        [JsonProperty(PropertyName = "policyOverride")]
        public PolicyOverride PolicyOverride { get; set; }

        [JsonProperty(PropertyName = "notes")]
        public List<object> Notes { get; set; }

        [JsonProperty(PropertyName = "workItems")]
        public List<WorkItemInfo> WorkItems { get; set; }

        [JsonProperty(PropertyName = "changes")]
        public List<VersionControlItemChange> Changes { get; set; }
    }

    public class ChangesetLink : ObjectLink
    {
        [JsonProperty(PropertyName = "changes")]
        public ObjectLink Changes { get; set; }

        [JsonProperty(PropertyName = "workItems")]
        public ObjectLink WorkItems { get; set; }

        [JsonProperty(PropertyName = "author")]
        public ObjectLink Author { get; set; }

        [JsonProperty(PropertyName = "checkedInBy")]
        public ObjectLink CheckedInBy { get; set; }
    }

    [DebuggerDisplay("{Id}")]
    public class Changeset : BaseObject<ChangesetLink>
    {
        [JsonProperty(PropertyName = "changesetId")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "author")]
        public UserIdentity Author { get; set; }

        [JsonProperty(PropertyName = "checkedInBy")]
        public UserIdentity CheckedInBy { get; set; }

        [JsonProperty(PropertyName = "createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty(PropertyName = "comment")]
        public string Comment { get; set; }

        [JsonProperty(PropertyName = "commentTruncated")]
        public bool? CommentTruncated { get; set; }

        [JsonProperty(PropertyName = "workItems")]
        public List<WorkItemInfo> WorkItems { get; set; }
    }
}
