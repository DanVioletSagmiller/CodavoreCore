// <copyright file="ObservableNode.cs" company="Codavore, LLC">
//     Copyright (c) Codavore, LLC. All rights reserved.
// </copyright>

namespace Codavore.Core
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public interface IObservableNode
    {
        Guid GetGuid();

        T GetValue<T>();
        
        void SetValue(object value);
        
        void StartListening(Action listener);
        
        void StopListening(Action listener);
        
        IObservableNode GetParent();

        string GetName();

        IObservableNode GetChild(string name);

        IObservableNode GetChildFromPath(string path);

        IEnumerable<IObservableNode> GetChildren();

        void SaveToJson(JsonWriter writer);

        void LoadFromJson(JsonTextReader reader, bool overwrite = false);
    }

    public class ObservableNode : IObservableNode
    {
        [JsonProperty]
        private object Value;

        [JsonIgnore]
        private Guid Guid;

        [JsonIgnore]
        private Action OnValueChange = () => { };

        [JsonIgnore]
        private IObservableNode Parent = null;

        [JsonProperty]
        private string Name = "";

        [JsonProperty]
        private Dictionary<string, IObservableNode> Children
            = new Dictionary<string, IObservableNode>();

        public ObservableNode(string name, IObservableNode parent)
        {
            this.Name = name;
            this.Parent = parent;
            this.Guid = Guid.NewGuid();
        }

        public ObservableNode(string name, IObservableNode parent, Guid guid)
        {
            this.Name = name;
            this.Parent = parent;
            this.Guid = guid;
        }

        public T GetValue<T>()
        {
            return (T)this.Value;
        }

        public void SetValue(object value)
        {
            if (object.Equals(value, this.Value))
            {
                return;
            }

            this.Value = value;
            this.OnValueChange();
        }

        public void StartListening(Action listener)
        {
            this.OnValueChange += listener;
        }

        public void StopListening(Action listener)
        {
            this.OnValueChange -= listener;
        }

        public IObservableNode GetParent()
        {
            return this.Parent;
        }

        public string GetName()
        {
            return this.Name;
        }

        public IEnumerable<IObservableNode> GetChildren()
        {
            foreach(var child in this.Children)
            {
                yield return child.Value;
            }
        }

        public IObservableNode GetChild(string name)
        {
            IObservableNode child = null;
            var childAlreadyExists = this.Children.ContainsKey(name);

            if (!childAlreadyExists)
            {
                child = new ObservableNode(name, this);
                this.Children.Add(name, child);
            }
            else
            {
                child = this.Children[name];
            }

            return child;
        }

        public IObservableNode GetChildFromPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return GetChild(path);
            }

            var isNotPath = !path.Contains(CodavoreCoreConstants.PathSeparaterString);
            if (isNotPath)
            {
                return GetChild(path);
            }

            var segments = path.Split(CodavoreCoreConstants.PathSeparaterCharArray);
            IObservableNode node = this;
            foreach(var segment in segments)
            {
                node = node.GetChild(segment);
            }
            
            return node;
        }

        public void SaveToJson(JsonWriter writer)
        {
            writer.WriteStartObject();
            // name, .type, .value, .children (then lead to <name repeating)
            if (this.Value != null)
            {
                writer.WritePropertyName("type");
                writer.WriteValue(this.Value.GetType().FullName);

                writer.WritePropertyName("value");
                JsonSerializer.CreateDefault().Serialize(writer, this.Value);
            }

            writer.WritePropertyName("childCount");
            writer.WriteValue(this.Children.Count);
            if (this.Children.Count > 0)
            {
                writer.WritePropertyName("children");
                writer.WriteStartArray();
                foreach (var child in this.Children.Values)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("name");
                    writer.WriteValue(child.GetName());
                    writer.WritePropertyName("data");
                    child.SaveToJson(writer);
                }

                writer.WriteEndArray();
            }
            writer.WriteEndObject();
        }
        private static JObject GetChild(
            JObject jObj, 
            string key)
        {
            var foundObj = (JObject)jObj
                .Children()
                .First((o) =>
            {
                var p = (JProperty)o.Parent;
                return p.Name == "value";
            }); 

            return foundObj;
        }
        public void LoadFromJson(JsonTextReader reader, bool overwrite = false)
        {
            reader.Validate(JsonToken.StartObject, "'node' object not started correctly.");
            reader.Read();

            if (reader.TokenType == JsonToken.EndObject) return;

            reader.Validate(JsonToken.PropertyName, read: false);
            var propName = (string)reader.Value;
            if (propName == "type")
            {
                reader.Read();
                // type and value exist
                var objectTypeName = (string)reader.Value;
                var objectType = Type.GetType(objectTypeName);

                reader.Validate(JsonToken.PropertyName, name: "value");
                reader.Read(); // starts the value, so the next line can work
                var value = JsonSerializer.CreateDefault().Deserialize(reader, objectType);
                if (overwrite)
                {
                    this.SetValue(value);
                }
                reader.Read();
            }

            if (reader.TokenType == JsonToken.EndObject)
            {
                return;
            }

            reader.Validate(JsonToken.PropertyName, name: "childCount", read: false);
            var childCount = reader.ReadAsInt32();
            Debug.Log("C#: " + childCount);

            //if (reader.TokenType == JsonToken.EndObject)
            if (reader.Validate(JsonToken.EndObject, errorOnFailure: false))
            {
                return;
            }

            reader.Validate(JsonToken.PropertyName, name: "children", read: false);
            reader.Validate(JsonToken.StartArray);
            for (int i = 0; i < childCount; i++)
            {
                reader.Validate(JsonToken.StartObject);

                reader.Validate(JsonToken.PropertyName, name: "name");
                reader.Validate(JsonToken.String);
                var childName = (string)reader.Value;

                var child = this.GetChild(childName);
                reader.Validate(JsonToken.PropertyName, name: "data");

                child.LoadFromJson(reader, overwrite);

                reader.Validate(JsonToken.EndObject);
            }

            reader.Validate(JsonToken.EndArray);
            reader.Validate(JsonToken.EndObject);

        }

        public Guid GetGuid()
        {
            throw new NotImplementedException();
        }
    }
}
