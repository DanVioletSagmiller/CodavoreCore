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

    }

    public class ObservableRoot : IObservableRoot
    {
        [JsonProperty]
        private Dictionary<string, IObservableNode> RootNodes
            = new Dictionary<string, IObservableNode>();

        private Action<string> AttemptingToLoadNode = (s) => { };

        public IObservableNode GetNode(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            if (path.Contains(CodavoreCoreConstants.PathSeparaterString))
            {
                var pathElements = path.Split(CodavoreCoreConstants.PathSeparaterCharArray);
                var node = GetRootNode(pathElements[0]);
                for(int i = 1; i < pathElements.Length; i++)
                {
                    node = node.GetChild(pathElements[i]);
                }

                return node;
            }
            else
            {
                return GetRootNode(path);
            }
        }

        private IObservableNode GetRootNode(string name)
        {
            if (this.RootNodes.ContainsKey(name))
            {
                return this.RootNodes[name];
            }

            this.AttemptingToLoadNode(name);

            if (this.RootNodes.ContainsKey(name))
            {
                return this.RootNodes[name];
            }

            IObservableNode rootNode = new ObservableNode(name, null);
            this.RootNodes.Add(name, rootNode);
            return rootNode;
        }

        public IEnumerable<IObservableNode> GetRootNodes()
        {
            foreach (var node in this.RootNodes.Values)
            {
                yield return node;
            }
        }

        public void StartListeningToLoadRequest(Action<string> loadHandler)
        {
            this.AttemptingToLoadNode += loadHandler;
        }

        public void StopListeningToLoadRequest(Action<string> loadHandler)
        {
            this.AttemptingToLoadNode -= loadHandler;
        }

        public bool RootNodeExists(string name)
        {
            return this.RootNodes.ContainsKey(name);
        }
        public string SaveRootNode(string name)
        {
            var node = GetRootNode(name);
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                writer.WriteStartObject();
                writer.WritePropertyName("version");
                writer.WriteValue("1");
                writer.WritePropertyName("name");
                writer.WriteValue(name);
                writer.WritePropertyName("node");
                node.SaveToJson(writer);
                writer.WriteEndObject();
            }
            return sb.ToString();
        }


        //        public string SaveRootNode(string name)
        //        {
        //            var node = GetRootNode(name);
        //            StringBuilder sb = new StringBuilder();
        //            StringWriter sw = new StringWriter(sb);

        //            using (JsonWriter writer = new JsonTextWriter(sw))
        //            {
        //                writer.Formatting = Formatting.Indented;
        //                writer.WriteStartObject();
        //                writer.WritePropertyName("version");
        //                writer.WriteValue("1");
        //                writer.WritePropertyName("name");
        //                writer.WriteValue(name);
        //                writer.WritePropertyName("node");
        //                node.SaveToJson(writer);
        //                writer.WriteEndObject();
        //            }

        //            return sb.ToString();
        //        }

        //#warning //TODO: RootNode needs an optional field to Overwrite existing fields and data, or to just add new items.  (I.e. reset level vs maintaining data.)
        //        // All Events should be retained.
        //        public void LoadRootNode(string json, bool overwrite = true)
        //        {
        //            // node, .type, .value, .children (then lead to <name repeating)
        //            var reader = new JsonTextReader(new StringReader(json));
        //            reader.Validate(JsonToken.StartObject, "Missing starting '{'");
        //            reader.Validate(JsonToken.PropertyName, "First property must be 'version'", "version");
        //            reader.Validate(JsonToken.String, "Improper JSON version.", "1");
        //            reader.Validate(JsonToken.PropertyName, "'name' should be the second property.", "name");

        //            reader.Validate(JsonToken.String, "'name' property should have had a string value.");
        //            var name = (string)reader.Value;

        //            reader.Validate(JsonToken.PropertyName, "'node' property should have been the third.", "node");
        //            this.GetNode(name).LoadFromJson(reader, overwrite);

        //            reader.Validate(JsonToken.EndObject, "Improper closing on node");
        //        }

        public void DeleteRootNode(string name)
        {
            this.RootNodes.Remove(name);
        }
    }
}
