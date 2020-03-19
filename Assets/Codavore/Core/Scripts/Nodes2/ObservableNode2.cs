//// <copyright file="ObservableNode.cs" company="Codavore, LLC">
////     Copyright (c) Codavore, LLC. All rights reserved.
//// </copyright>

//namespace Codavore.Core
//{
//    using System;
//    using System.Collections.Generic;

//    public interface IObservableNode
//    {
//        string Name { get; }

//        void Trigger();

//        T GetValue<T>();

//        void SetValue(object obj);

//        IObservableNode Parent { get; }

//        IObservableNode GetChild(string name);

//        IEnumerable<IObservableNode> GetChildren();

//        IObservableNode GetChildFromPath(string path);

//        void StartListener(Action onTrigger);

//        void StopListener(Action onTrigger);
//    }

//    public class ObservableNode : IObservableNode
//    {
//        private Dictionary<string, IObservableNode> Children
//            = new Dictionary<string, IObservableNode>();

//        private object _Value;
//        private System.Action OnChange = () => { };
//        public string Name { get; private set; }

//        public void Trigger()
//        {
//            this.OnChange();
//        }

//        public T GetValue<T>()
//        {
//            return (T)this._Value;
//        }

//        public void SetValue(object obj)
//        {

//            if (object.Equals(this._Value, obj))
//            {
//                return;
//            }

//            this._Value = obj;
//            this.Trigger();
//        }

//        public IObservableNode Parent { get; private set; }

//        public ObservableNode(string name, IObservableNode parent)
//        {
//            this.Name = name;
//            this.Parent = parent;
//        }

//        public IObservableNode GetChild(string name)
//        {
//            if (this.Children.ContainsKey(name))
//            {
//                return this.Children[name];
//            }

//            return GenerateNewChild(name);
//        }

//        private IObservableNode GenerateNewChild(string name)
//        {
//            var node = new ObservableNode(name, this);
//            this.Children.Add(name, node);
//            return node;
//        }

//        public IEnumerable<IObservableNode> GetChildren()
//        {
//            foreach(var child in this.Children)
//            {
//                yield return child.Value;
//            }
//        }

//        public IObservableNode GetChildFromPath(string path)
//        {
//            IObservableNode node = this;
//            foreach (var nodeName in path.Split(CodavoreConstants.NodeParseCharacter))
//            {
//                node = node.GetChild(nodeName);
//            }

//            return node;
//        }

//        public void StartListener(Action onTrigger)
//        {
//            this.OnChange += onTrigger;
//        }

//        public void StopListener(Action onTrigger)
//        {
//            this.OnChange -= onTrigger;
//        }
//    }
//}