// <copyright file="ObservableRoot.cs" company="Codavore, LLC">
//     Copyright (c) Codavore, LLC. All rights reserved.
// </copyright>

namespace Codavore.Core
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
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
            // node, .type, .value, .children (then lead to <name repeating)
            return JsonConvert.SerializeObject(GetNode(name));
        }

#warning //TODO: RootNode needs an optional field to Overwrite existing fields and data, or to just add new items.  (I.e. reset level vs maintaining data.)
        // All Events should be retained.
        public void LoadRootNode(string json)
        {
            // node, .type, .value, .children (then lead to <name repeating)
            var node = JsonConvert.DeserializeObject<ObservableNode>(json);
            this.RootNodes.Add(node.GetName(), node);
        }

        public void DeleteRootNode(string name)
        {
            this.RootNodes.Remove(name);
        }
    }
}
