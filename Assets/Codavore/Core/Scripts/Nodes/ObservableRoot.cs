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
        private Dictionary<Guid, IObservableNode> Nodes = new Dictionary<Guid, IObservableNode>();

        public IObservableNode GetNode(Guid guid)
        {
            if (this.Nodes.ContainsKey(guid))
            {
                return this.Nodes[guid];
            }

            return null;
        }

        public List<string> GetPaths()
        {
            var paths = new List<string>();
            foreach(var pair in this.Nodes)
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
            var nodes = GetNodes(path, withLineage: true);
            // begin jsonWriter
            // Start Array
            // Save All
            // return string
        }

        public string LoadLineage(string json)
        {

        }
    }
}
