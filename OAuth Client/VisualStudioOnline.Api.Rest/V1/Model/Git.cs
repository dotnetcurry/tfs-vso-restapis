using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace VisualStudioOnline.Api.Rest.V1.Model
{
    [DebuggerDisplay("{Name}")]
    public class Repository : ObjectWithId<string>
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("project")]
        public TeamProject Project { get; set; }

        [JsonProperty("defaultBranch")]
        public string DefaultBranch { get; set; }

        [JsonProperty("remoteUrl")]
        public string RemoteUrl { get; set; }
    }

    [DebuggerDisplay("{Name}")]
    public class GitReference : BaseObject
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("objectId")]
        public string ObjectId { get; set; }
    }

    [DebuggerDisplay("{Name}")]
    public class GitUser
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }
    }

    [DebuggerDisplay("{CommitId}")]
    public class Commit : BaseObject
    {
        [JsonProperty("commitId")]
        public string CommitId { get; set; }

        [JsonProperty("author")]
        public GitUser Author { get; set; }

        [JsonProperty("committer")]
        public GitUser Committer { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("parents")]
        public IList<string> Parents { get; set; }

        [JsonProperty("treeId")]
        public string TreeId { get; set; }
    }

    [DebuggerDisplay("{Name}")]
    public class GitBranchInfo
    {
        [JsonProperty("commit")]
        public Commit Commit { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("aheadCount")]
        public int AheadCount { get; set; }

        [JsonProperty("behindCount")]
        public int BehindCount { get; set; }

        [JsonProperty("isBaseVersion")]
        public bool IsBaseVersion { get; set; }
    }
}
