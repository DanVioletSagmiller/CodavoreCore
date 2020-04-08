// <copyright file="ObservableRoot.cs" company="Codavore, LLC">
//     Copyright (c) Codavore, LLC. All rights reserved.
// </copyright>

namespace Codavore.Core
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEngine;

    public interface IObservableRoot
    {
        IObservableNode GetNode(Guid guid);
        List<string> GetPaths();
        List<string> GetRootPaths();
        List<IObservableNode> GetNodes(string path, bool withLineage = false);
        string SaveLineage(string path);
        void LoadLineage(string json);
    }

    public class ObservableRoot : IObservableRoot
    {
        private Dictionary<Guid, IObservableNode> Nodes = new Dictionary<Guid, IObservableNode>();

        public IObservableNode GetNode(Guid guid)
        {
            if (this.Nodes.ContainsKey(guid))
            {
                return this.Nodes[guid];
            }

            var node = new ObservableNode(guid);
            this.Nodes.Add(guid, node);
            return node;
        }

        public List<string> GetPaths()
        {
            var paths = new List<string>();
            foreach (var pair in this.Nodes)
            {
                var path = pair.Value.GetPath();
                if (paths.Contains(path))
                {
                    continue;
                }

                paths.Add(path);
            }

            return paths;
        }

        public List<string> GetRootPaths()
        {
            var rootPaths = new List<string>();
            foreach (var path in GetPaths())
            {
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }

                var rootPath = path.Split('/')[0];
                if (string.IsNullOrEmpty(rootPath) || rootPaths.Contains(rootPath))
                {
                    continue;
                }

                rootPaths.Add(rootPath);
            }

            return rootPaths;
        }

        public List<IObservableNode> GetNodes(string path, bool withLineage = false)
        {
            var nodes = new List<IObservableNode>();
            if (withLineage)
            {
                foreach (var pair in this.Nodes)
                {
                    if (pair.Value.GetPath().StartsWith(path))
                    {
                        nodes.Add(pair.Value);
                    }
                }
            }
            else
            {
                foreach (var pair in this.Nodes)
                {
                    if (pair.Value.GetPath() == path)
                    {
                        nodes.Add(pair.Value);
                    }
                }
            }

            return nodes;
        }

        public string SaveLineage(string path)
        {
            var serializer = JsonSerializer.CreateDefault();
            var nodes = GetNodes(path, withLineage: true);
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                writer.WriteStartObject();

                writer.WritePropertyName("version");
                writer.WriteValue("1");

                writer.WritePropertyName("nodes");
                writer.WriteStartArray();

                foreach (var node in nodes)
                {
                    writer.WriteStartObject();

                    writer.WritePropertyName("guid");
                    writer.WriteValue(node.GetGuid().ToString());

                    writer.WritePropertyName("name");
                    writer.WriteValue(node.GetName());

                    writer.WritePropertyName("path");
                    writer.WriteValue(node.GetName());

                    var obj = node.GetValue<object>();

                    writer.WritePropertyName("type");
                    if (obj != null)
                    {
                        writer.WriteValue(obj.GetType().FullName);
                        writer.WritePropertyName("value");
                        serializer.Serialize(writer, obj);
                    }
                    else
                    {
                        writer.WriteValue("none");
                    }

                    writer.WriteEndObject();
                }

                writer.WriteEndArray();
                writer.WriteEndObject();
            }

            return sb.ToString();
        }

        public void LoadLineage1(string json)
        {
            var serializer = JsonSerializer.CreateDefault();
            var reader = new JsonTextReader(new StringReader(json));
            if (!reader.Validate(JsonToken.StartObject))
            {
                Debug.LogWarning("LoadLineage given JSON missing opening '{'. Json = " + json);
                return;
            }

            if (!reader.Validate(JsonToken.PropertyName, name: "version"))
            {
                Debug.LogWarning("LoadLineage given JSON without starting version property. Json = " + json);
                return;
            }

            if (!reader.Validate(JsonToken.String, name: "1"))
            {
                Debug.LogWarning("LoadLineage given JSON without starting version of '1'. Json = " + json);
                return;
            }

            if (!reader.Validate(JsonToken.PropertyName, name: "nodes"))
            {
                Debug.LogWarning("LoadLineage given JSON without starting nodes property. Json = " + json);
                return;
            }


            if (!reader.Validate(JsonToken.StartArray))
            {
                Debug.LogWarning("LoadLineage given JSON with starting nodes property, but it was not an array. Json = " + json);
                return;
            }

            var hasObj = reader.Validate(JsonToken.StartObject); // if false, it means .EndObject
            int index = 0;
            while (hasObj)
            {
                index++;

                if (!reader.Validate(JsonToken.PropertyName, name: "guid"))
                {
                    Debug.LogWarning("LoadLineage given JSON, but node " + index + " is missing its guid property. Json = " + json);
                    return;
                }

                var guid = Guid.Parse(reader.ReadAsString());

                if (!reader.Validate(JsonToken.PropertyName, name: "name"))
                {
                    Debug.LogWarning("LoadLineage given JSON, but node " + index + " is missing its name property. Json = " + json);
                    return;
                }

                var name = reader.ReadAsString();

                if (!reader.Validate(JsonToken.PropertyName, name: "path"))
                {
                    Debug.LogWarning("LoadLineage given JSON, but node " + index + " is missing its path property. Json = " + json);
                    return;
                }

                var path = reader.ReadAsString();

                if (!reader.Validate(JsonToken.PropertyName, name: "type"))
                {
                    Debug.LogWarning("LoadLineage given JSON, but node " + index + " is missing its type property. Json = " + json);
                    return;
                }

                var typeName = reader.ReadAsString();
                var hasType = typeName != "none";
                if (hasType)
                {
                    var type = Type.GetType(typeName);

                    if (!reader.Validate(JsonToken.PropertyName, name: "value"))
                    {
                        Debug.LogWarning("LoadLineage given JSON, but node " + index + " is missing its value property, despite having a type property not set to 'none'. Json = " + json);
                        return;
                    }

                    var obj = serializer.Deserialize(reader, type);
                    var node = this.GetNode(guid);
                    node.Reset(name, path, obj);
                }
                else
                {
                    var node = this.GetNode(guid);
                    node.Reset(name, path, null);
                }

                if (!reader.Validate(JsonToken.EndObject))
                {
                    Debug.LogWarning("LoadLineage given JSON, but node " + index + " is missing its '}'. Json = " + json);
                    return;
                }

                hasObj = reader.Validate(JsonToken.StartObject);
            }
        }

        public void LoadLineage(string json)
        {
            var jObj = JObject.Parse(json);
            var version = jObj.SelectToken("version");
            if (version == null)
            {
                Debug.LogWarning("LoadLineage given JSON without starting version property. Json = " + json);
                return;
            }

            if (version.Value<int>() != 1)
            {
                Debug.LogWarning("LoadLineage given JSON without starting version of '1'. Json = " + json);
                return;
            }

            var nodes = jObj["nodes"].Children();
            foreach (var child in nodes)
            {
                var guidContent = child["guid"].Value<string>();
                var guid = Guid.Parse(guidContent);

                var name = child["name"].Value<string>();
                var path = child["path"].Value<string>();
                var typeName = child["type"].Value<string>();
                
                Type type = null;
                object value = null;

                if (typeName != "none")
                {
                    type = Type.GetType(typeName);
                    value = child["value"].ToObject(type);
                }

                var node = this.GetNode(guid);
                node.Reset(name, path, value);
            }
            
        }
    }
}
