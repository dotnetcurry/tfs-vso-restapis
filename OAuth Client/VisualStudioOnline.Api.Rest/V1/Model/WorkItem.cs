using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;

namespace VisualStudioOnline.Api.Rest.V1.Model
{
    public enum TopologyType
    {
        dependency,
        network,
        tree
    }

    public enum OperationType
    {
        add,
        remove,
        replace,        
        test
    }
   
    public class RelationTypeAttributes
    {
        [JsonProperty(PropertyName = "usage")]
        public string Usage { get; set; }

        [JsonProperty(PropertyName = "editable")]
        public bool Editable { get; set; }

        [JsonProperty(PropertyName = "enabled")]
        public bool Enabled { get; set; }

        [JsonProperty(PropertyName = "acyclic")]
        public bool Acyclic { get; set; }

        [JsonProperty(PropertyName = "directional")]
        public bool Directional { get; set; }

        [JsonProperty(PropertyName = "singleTarget")]
        public bool SingleTarget { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "topology")]
        public TopologyType Topology { get; set; }
    }

    [DebuggerDisplay("{ReferenceName}")]
    public class WorkItemRelationType : BaseObject
    {
        [JsonProperty(PropertyName = "referenceName")]
        public string ReferenceName { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "attributes")]
        public RelationTypeAttributes Attributes { get; set; }
    }

    [DebuggerDisplay("{Name}")]
    public class User : ObjectWithId<string>
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }

    [DebuggerDisplay("{Value}")]
    public class HistoryComment : BaseObject
    {
        [JsonProperty(PropertyName = "rev")]
        public int Rev { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "revisedBy")]
        public User RevisedBy { get; set; }

        [JsonProperty(PropertyName = "revisedDate")]
        public DateTime RevisedDate { get; set; }
    }

    [DebuggerDisplay("{Id:Name}")]
    public class RelationAttributes
    {
        [JsonProperty(PropertyName = "authorizedDate")]
        public DateTime AuthorizedDate { get; set; }

        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "resourceCreatedDate")]
        public DateTime ResourceCreatedDate { get; set; }

        [JsonProperty(PropertyName = "resourceModifiedDate")]
        public DateTime ResourceModifiedDate { get; set; }

        [JsonProperty(PropertyName = "revisedDate")]
        public DateTime RevisedDate { get; set; }

        [JsonProperty(PropertyName = "resourceSize")]
        public int ResourceSize { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "isLocked")]
        public bool? IsLocked { get; set; }

        [JsonProperty(PropertyName = "comment")]
        public string Comment { get; set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ AuthorizedDate.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var attributes = obj as RelationAttributes;
            if (attributes == null) return false;

            return Id == attributes.Id && AuthorizedDate == attributes.AuthorizedDate;
        }
    }

    [DebuggerDisplay("{Rel}")]
    public class WorkItemRelation : BaseObject
    {
        [JsonIgnore]
        internal int Index { get; set; }

        [JsonProperty(PropertyName = "rel")]
        public string Rel { get; set; }

        [JsonProperty(PropertyName = "attributes")]
        public RelationAttributes Attributes { get; set; }

        [JsonProperty(PropertyName = "source")]
        public WorkItem Source { get; set; }

        [JsonProperty(PropertyName = "target")]
        public WorkItem Target { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ (Rel != null? Rel.GetHashCode() : 1) ^ (Attributes != null? Attributes.GetHashCode() : 1);
        }

        public override bool Equals(object obj)
        {
            var relation = obj as WorkItemRelation;
            if (relation == null) return false;
            
            return relation.Rel == Rel && relation.Attributes == Attributes && base.Equals(obj);
        }
    }

    [DebuggerDisplay("{Id:Rev}")]
    public abstract class WorkItemCore : ObjectWithId<int>
    {
        [JsonProperty(PropertyName = "rev")]
        public int Rev { get; set; }
    }
    
    public class WorkItem : WorkItemCore
    {
        internal List<FieldUpdate> FieldUpdates = new List<FieldUpdate>();
        internal List<RelationUpdate> RelationUpdates = new List<RelationUpdate>();

        [JsonProperty(PropertyName = "relations")]
        public ObservableCollection<WorkItemRelation> Relations { get; set; }

        [JsonProperty(PropertyName = "fields")]
        public ObservableDictionary<string, object> Fields;

        [JsonProperty(PropertyName = "_links")]
        public WorkItemLink References;

        public WorkItem()
        {
            Relations = new ObservableCollection<WorkItemRelation>();
            Fields = new ObservableDictionary<string, object>();

            Relations.CollectionChanged += OnRelations_CollectionChanged;
            Fields.CollectionChanged += OnFields_CollectionChanged;
        }

        private void OnFields_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add ||
                e.Action == NotifyCollectionChangedAction.Remove ||
                e.Action == NotifyCollectionChangedAction.Replace)
            {
                //TODO: NotifyCollectionChangedAction.Replace

                if (e.NewItems != null)
                {
                    foreach (KeyValuePair<string, object> newfield in e.NewItems)
                    {
                        var existingUpdate = FieldUpdates.FirstOrDefault(fu => fu.Name == newfield.Key);
                        if (existingUpdate != null)
                        {
                            FieldUpdates.Remove(existingUpdate);
                        }

                        FieldUpdates.Add(new FieldUpdate(newfield.Key, newfield.Value, (OperationType)e.Action /*TODO*/));
                    }
                }

                if (e.Action != NotifyCollectionChangedAction.Replace && e.OldItems != null)
                {
                    foreach (KeyValuePair<string, object> removedField in e.OldItems)
                    {
                        var existingUpdate = FieldUpdates.FirstOrDefault(fu => fu.Name == removedField.Key);
                        if (existingUpdate != null)
                        {
                            FieldUpdates.Remove(existingUpdate);
                        }

                        FieldUpdates.Add(new FieldUpdate(removedField.Key, (OperationType)e.Action /*TODO*/));
                    }
                }
            }
        }

        private void OnRelations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add || 
                e.Action == NotifyCollectionChangedAction.Remove || 
                e.Action == NotifyCollectionChangedAction.Replace)
            {
                if (e.NewItems != null)
                {
                    foreach (WorkItemRelation newRelation in e.NewItems)
                    {
                        var existingUpdate = RelationUpdates.FirstOrDefault(ru => ru.Value == newRelation);
                        if (existingUpdate != null)
                        {
                            if (existingUpdate.Operation == OperationType.remove)
                            {
                                RelationUpdates.Remove(existingUpdate);
                            }
                        }
                        else
                        {
                            RelationUpdates.Add(new RelationUpdate(newRelation, OperationType.add));
                        }
                    }
                }

                if (e.OldItems != null)
                {
                    foreach (WorkItemRelation oldRelation in e.OldItems)
                    {
                        var existingUpdate = RelationUpdates.FirstOrDefault(ru => ru.Value == oldRelation);
                        if (existingUpdate != null)
                        {
                            if (existingUpdate.Operation == OperationType.add)
                            {
                                RelationUpdates.Remove(existingUpdate);
                            }
                        }
                        else
                        {
                            RelationUpdates.Add(new RelationUpdate(oldRelation, OperationType.remove));
                        }
                    }
                }
            }
            else if(e.Action == NotifyCollectionChangedAction.Reset)
            {
                // Remove all links
                RelationUpdates.Clear();
                RelationUpdates.Add(new RelationUpdate() { Operation = OperationType.remove });                
            }
        }
    }

    public class WorkItemLink : ObjectLink
    {
        [JsonProperty(PropertyName = "workItemUpdates")]
        public ObjectLink Updates { get; set; }

        [JsonProperty(PropertyName = "workItemRevisions")]
        public ObjectLink Revisions { get; set; }

        [JsonProperty(PropertyName = "workItemHistory")]
        public ObjectLink History { get; set; }

        [JsonProperty(PropertyName = "html")]
        public ObjectLink Html { get; set; }

        [JsonProperty(PropertyName = "workItemType")]
        public ObjectLink Type { get; set; }

        [JsonProperty(PropertyName = "fields")]
        public ObjectLink Fields { get; set; }
    }

    public class RelationChanges
    {
        [JsonProperty(PropertyName = "added")]
        public List<WorkItemRelation> AddedRelations { get; set; }

        [JsonProperty(PropertyName = "removed")]
        public List<WorkItemRelation> RemovedRelations { get; set; }
    }

    public class WorkItemUpdate : WorkItemCore
    {
        [JsonProperty(PropertyName = "revisedBy")]
        public User RevisedBy { get; set; }

        [JsonProperty(PropertyName = "revisedDate")]
        public DateTime RevisedDate { get; set; }

        [JsonProperty(PropertyName = "relations")]
        public RelationChanges Changes { get; set; }

        [JsonProperty(PropertyName = "fields")]
        public Dictionary<string, FieldChange> FieldChanges;
    }

    [DebuggerDisplay("{OldValue}->{NewValue}")]
    public class FieldChange
    {
        [JsonProperty(PropertyName = "oldValue")]
        public object OldValue { get; set; }

        [JsonProperty(PropertyName = "newValue")]
        public object NewValue { get; set; }
    }

    [DebuggerDisplay("{Path}")]
    internal abstract class Update
    {
        [JsonProperty(PropertyName = "op")]
        [JsonConverter(typeof(StringEnumConverter))]
        public OperationType Operation { get; set; }

        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        [JsonProperty(PropertyName = "value", NullValueHandling = NullValueHandling.Ignore)]
        public object Value { get; set; }
    }
    
    internal class FieldUpdate : Update
    {
        [JsonIgnore]
        public string Name { get; set; }

        public FieldUpdate(string referenceName, OperationType operation)
        {
            Operation = operation;
            Name = referenceName;
            Path = string.Format("/fields/{0}", referenceName);
        }

        public FieldUpdate(string referenceName, object value, OperationType operation = OperationType.add) : this(referenceName, operation)
        {            
            Value = value;
        }
    }

    internal class RelationUpdate : Update
    {
        public RelationUpdate()
        {
            Path = "/relations/-";
        }

        public RelationUpdate(WorkItemRelation relation, OperationType operation)
        {
            Operation = operation;
            Path = operation == OperationType.add ? "/relations/-" : string.Format("/relations/{0}", relation.Index);

            if (operation != OperationType.remove)
            {
                var attributeDictionary = new Dictionary<string, object>();
                if (relation.Attributes.Comment != null) attributeDictionary.Add("comment", relation.Attributes.Comment);
                if (relation.Attributes.IsLocked != null) attributeDictionary.Add("isLocked", relation.Attributes.IsLocked.Value);

                Value = new { rel = relation.Rel, url = relation.Url, attributes = attributeDictionary };
            }
        }
    }
}
