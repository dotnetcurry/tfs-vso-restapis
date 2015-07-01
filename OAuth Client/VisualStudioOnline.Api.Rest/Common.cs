using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;

namespace VisualStudioOnline.Api.Rest
{
    [DebuggerDisplay("{Count}")]
    public class JsonCollection<T> where T : class
    {
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }

        [JsonProperty(PropertyName = "value")]
        public List<T> Items { get; set; }

        public T this[int index]
        {
            get
            {
                return Items[index];
            }
            set
            {
                Items[index] = value;
            }
        }
    }

    [DebuggerDisplay("{Url}")]
    public abstract class BaseObject
    {
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        public override int GetHashCode()
        {
            return Url != null ? Url.GetHashCode() : base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as BaseObject;
            if (other == null) return false;

            return Url == other.Url;
        }

        public override string ToString()
        {
            return Url != null ? Url.ToString() : base.ToString();
        }
    }

    public abstract class BaseObject<T> : BaseObject
    {
        [JsonProperty(PropertyName = "_links")]
        public T ObjectLinks { get; set; }
    }

    [DebuggerDisplay("{Id}")]
    public class ObjectWithId<TId, TLinks> : BaseObject<TLinks>
    {
        [JsonProperty(PropertyName = "id")]
        public TId Id { get; set; }
    }

    [DebuggerDisplay("{Id}")]
    public class ObjectWithId<T> : BaseObject
    {
        [JsonProperty(PropertyName = "id")]
        public T Id { get; set; }
    }

    [DebuggerDisplay("{Href}")]
    public class ObjectLink
    {
        [JsonProperty(PropertyName = "href")]
        public string Href { get; set; }
    }

    [DebuggerDisplay("{Self.Href}")]
    public class SelfLink
    {
        [JsonProperty(PropertyName = "self")]
        public ObjectLink Self { get; set; }
    }

    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged
    {
        private Dictionary<TKey, TValue> _internalDictionary = new Dictionary<TKey, TValue>();

        public void Add(TKey key, TValue value)
        {
            _internalDictionary.Add(key, value);
            OnCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value));
        }

        public bool ContainsKey(TKey key)
        {
            return _internalDictionary.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return _internalDictionary.Keys; }
        }

        public bool Remove(TKey key)
        {
            TValue item;

            if (_internalDictionary.TryGetValue(key, out item))
            {
                _internalDictionary.Remove(key);
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, item));
                return true;
            }

            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _internalDictionary.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get { return _internalDictionary.Values; }
        }

        public TValue this[TKey key]
        {
            get
            {
                return _internalDictionary[key];
            }
            set
            {
                TValue item;
                bool itemExists = _internalDictionary.TryGetValue(key, out item);

                _internalDictionary[key] = value;

                if(itemExists)
                {
                    OnCollectionChanged(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, value), new KeyValuePair<TKey, TValue>(key, item));
                }
                else
                {
                    OnCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value));
                }                
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_internalDictionary).Add(item);
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item);
        }

        public void Clear()
        {
            _internalDictionary.Clear();
            OnCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_internalDictionary).Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_internalDictionary).CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _internalDictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _internalDictionary.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _internalDictionary.GetEnumerator();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void OnCollectionChanged(NotifyCollectionChangedAction action)
        {
            if(CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action));
            }
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> changedItem)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, changedItem));
            }
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, newItem, oldItem));
            }
        }
    }
}
