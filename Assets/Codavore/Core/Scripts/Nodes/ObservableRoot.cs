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
        void DeleteLineage(string path, bool includeActions = false);
        void DeleteNode(Guid guid, bool includeActions = false);
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

                var rootPath = path.Split(CodavoreConstants.PathSeparaterChar)[0];
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


                var lights = GameObject.FindObjectsOfType<Light>();


            }

            return sb.ToString();
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

        public void DeleteLineage(string path, bool includeActions = false)
        {
            var nodes = this.GetNodes(path, withLineage: true);
            foreach(var node in nodes)
            {
                this.DeleteNode(node.GetGuid(), includeActions);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="includeActions">If set true, this will also delete nodes with listeners for changes.</param>
        public void DeleteNode(Guid guid, bool includeActions = false)
        {
            if (includeActions)
            {
                this.Nodes.Remove(guid);
                return;
            }

            var node = this.GetNode(guid);
            if (!node.HasListeners())
            {
                this.Nodes.Remove(guid);
            }
        }
    }
}
