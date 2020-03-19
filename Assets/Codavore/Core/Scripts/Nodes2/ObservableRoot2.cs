//// <copyright file="ObservableHelper.cs" company="Codavore, LLC">
////     Copyright (c) Codavore, LLC. All rights reserved.
//// </copyright>

//namespace Codavore.Core
//{
//    using Newtonsoft.Json;
//    using Newtonsoft.Json.Linq;
//    using System;
//    using System.Collections.Generic;

//    public class ObservableRoot 
//    {
//        // Stores a dictionary of starting nodes. We will assume the root element of a node will never have an event.
//        // Passes off the nodes at the beginning.
//        // named elements will be created via scriptables, and hard referenced. This class is not fully protected, but its weaknesses must be clarified.

//        private Dictionary<string, IObservableNode> Nodes
//            = new Dictionary<string, IObservableNode>();

//        private Dictionary<string, string> RootSourceFiles // used to store saved file names.
//            = new Dictionary<string, string>();

//        public IObservableNode GetNode(string path)
//        {
//            if (string.IsNullOrWhiteSpace(path))
//            {
//                return null;
//            }

//            var elements = path.Split(CodavoreConstants.NodeParseCharacter, StringSplitOptions.RemoveEmptyEntries);
//            var partCount = elements.Length;

//            if (partCount == 0)
//            {
//                return null;
//            }

//            if (partCount == 1)
//            {
//                return GetRootNode(elements[0]);
//            }

//            path = String.Join(
//                CodavoreConstants.NodeParseCharacter[0].ToString(), 
//                elements, 
//                1, 
//                elements.Length - 1);

//            return GetRootNode(elements[0]).GetChildFromPath(path);
//        }

//        private IObservableNode GetRootNode(string pathRoot)
//        {
//            if (Nodes.ContainsKey(pathRoot))
//            {
//                return Nodes[pathRoot];
//            }

//            IObservableNode node = null;

//            if (RootSourceFiles.ContainsKey(pathRoot))
//            {
//                JObject jo = new JObject(System.IO.File.ReadAllText(RootSourceFiles[pathRoot]));
//                node = new ObservableNode(pathRoot, null);
//            }
//            else
//            {
//                node = new ObservableNode(pathRoot, null);
//            }

//            Nodes[pathRoot] = node;
//            return node;

//        }

//        private void Populate(ObservableNode node, JObject jo)
//        {
//            // Structure: 
//            // Node.type (empty means no type)
//            // Node.data (content)
//            // Node.children (will not exist for no children)
//            var type = GetTypeFromNode(jo);
//            if (type == null)
//            {
//                return;
//            }

//            node.SetValue(GetObjectFromNode(jo, type));
//            var children = GetNodeChildren(jo);

//            foreach(var child in children)
//            {
//                string name = "";
//                name = child.ToObject<JProperty>().Name;
                
//                var childNode = node.GetChild(name);

//                Populate((ObservableNode)childNode, (JObject)child);
//            }
//        }

//        public void SaveToJson(string rootNode, JObject destination)
//        {

//        }

//        private JEnumerable<JToken> GetNodeChildren(JObject jo)
//        {
//            var childrenNode = jo["children"];
//            if (childrenNode == null)
//            {
//                return new JEnumerable<JToken>();
//            }

//            return childrenNode.Children();
//        }

//        private object GetObjectFromNode(JObject jo, Type type)
//        {
//            var dataNode = jo["data"];
//            if (dataNode == null)
//            {
//                return null;
//            }

//            return JsonConvert.DeserializeObject(dataNode.Value<string>());
//        }

//        private Type GetTypeFromNode(JObject jo)
//        {
//            var typeNode = jo["type"];
//            if (typeNode == null)
//            {
//                return null;
//            }

//            var typeString = typeNode.Value<string>();
//            if (String.IsNullOrWhiteSpace(typeString))
//            {
//                return null;
//            }

//            var type = Type.GetType(typeString);
//            return type;
//        }

//        private void SetNodeChildren(JObject jo, ObservableNode node)
//        {
//            var childrenNode = jo["children"];
//            if (childrenNode == null)
//            {
//                return new JEnumerable<JToken>();
//            }

//            return childrenNode.Children();
//        }

//        private void SetObjectFromNode(JObject jo, ObservableNode node)
//        {
//            var dataNode = jo["data"];
//            if (dataNode == null)
//            {
//                return null;
//            }

//            return JsonConvert.DeserializeObject(dataNode.Value<string>());
//        }

//        private void SetTypeFromNode(JObject jo, ObservableNode node)
//        {
//            var typeNode = jo["type"];
//            if (typeNode == null)
//            {
//                return null;
//            }

//            var typeString = typeNode.Value<string>();
//            if (String.IsNullOrWhiteSpace(typeString))
//            {
//                return null;
//            }

//            var type = Type.GetType(typeString);
//            return type;
//        }
//    }
//}