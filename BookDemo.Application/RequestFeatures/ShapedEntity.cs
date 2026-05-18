using BookDemo.Application.Models.LinkModels;
using System.Dynamic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BookDemo.Application.RequestFeatures
{

    /// <summary>
    /// Represents a dynamically shaped object used for Data Shaping.
    /// 
    /// WHY THIS CLASS EXISTS:
    /// 
    /// After applying data shaping (fields=...), we no longer return a fixed DTO.
    /// Instead, we return only selected properties dynamically.
    /// 
    /// ExpandoObject works fine for JSON, but:
    /// ❌ Not reliable for XML serialization
    /// ❌ Not flexible enough for custom formatters (CSV/XML)
    /// 
    /// This class solves that by:
    /// ✔ Acting like a dynamic object
    /// ✔ Storing values in a dictionary
    /// ✔ Giving full control over XML output
    /// ✔ Supporting HATEOAS (Links)
    /// </summary>
    public class ShapedEntity : DynamicObject, IDictionary<string, object?>, IXmlSerializable
    {
        private readonly IDictionary<string, object?> _values =
            new Dictionary<string, object?>();

        #region 🔹 DynamicObject Support

        /// <summary>
        /// Enables dynamic property read:
        /// Example: entity.Title
        /// </summary>
        public override bool TryGetMember(GetMemberBinder binder, out object? result)
        {
            return _values.TryGetValue(binder.Name, out result);
        }

        /// <summary>
        /// Enables dynamic property write:
        /// Example: entity.Title = "Clean Code"
        /// </summary>
        public override bool TrySetMember(SetMemberBinder binder, object? value)
        {
            _values[binder.Name] = value;
            return true;
        }

        #endregion

        #region 🔹 Indexer (dictionary-like access)

        public object? this[string key]
        {
            get => _values[key];
            set => _values[key] = value;
        }

        #endregion

        #region 🔹 IDictionary Implementation

        public ICollection<string> Keys => _values.Keys;
        public ICollection<object?> Values => _values.Values;
        public int Count => _values.Count;
        public bool IsReadOnly => _values.IsReadOnly;

        public void Add(string key, object? value) => _values.Add(key, value);
        public bool ContainsKey(string key) => _values.ContainsKey(key);
        public bool Remove(string key) => _values.Remove(key);
        public bool TryGetValue(string key, out object? value) => _values.TryGetValue(key, out value);

        public void Add(KeyValuePair<string, object?> item) => _values.Add(item);
        public void Clear() => _values.Clear();
        public bool Contains(KeyValuePair<string, object?> item) => _values.Contains(item);
        public void CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex) => _values.CopyTo(array, arrayIndex);
        public bool Remove(KeyValuePair<string, object?> item) => _values.Remove(item);

        public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() => _values.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _values.GetEnumerator();

        #endregion

        #region 🔹 XML Serialization (CRITICAL PART)

        public XmlSchema? GetSchema() => null;

        /// <summary>
        /// Not needed (only writing XML, not reading)
        /// </summary>
        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Custom XML output to properly serialize dynamic properties
        /// and especially HATEOAS Links.
        /// </summary>
        public void WriteXml(XmlWriter writer)
        {
            foreach (var kvp in _values)
            {
                WriteValueToXml(kvp.Key, kvp.Value, writer);
            }
        }

        /// <summary>
        /// Handles writing values to XML recursively.
        /// Supports:
        /// ✔ primitive values
        /// ✔ collections
        /// ✔ HATEOAS Links
        /// </summary>
        private void WriteValueToXml(string key, object? value, XmlWriter writer)
        {
            writer.WriteStartElement(key);

            if (value is IEnumerable<LinkDto> links)
            {
                foreach (var link in links)
                {
                    writer.WriteStartElement("Link");

                    writer.WriteElementString(nameof(link.Href), link.Href);
                    writer.WriteElementString(nameof(link.Method), link.Method);
                    writer.WriteElementString(nameof(link.Rel), link.Rel);

                    writer.WriteEndElement();
                }
            }
            else
            {
                writer.WriteString(value?.ToString());
            }

            writer.WriteEndElement();
        }

        #endregion
    }
}