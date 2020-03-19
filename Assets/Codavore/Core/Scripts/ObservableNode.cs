//// <copyright file="ObservableNode.cs" company="Codavore, LLC">
////     Copyright (c) Codavore, LLC. All rights reserved.
//// </copyright>

//namespace Codavore.Core
//{
//    using System;
//    using System.Collections.Generic;

//    public enum ObservableNodeTypes
//    {
//        HierarchyOnly,
//        Event,
//        Path,
//        Variable
//    }

//    public interface IObservableNode
//    {
//        string Name { get; }

//        void Trigger();
        
//        bool HasBeenSet { get; }

//        void Listen(System.Action onChange);

//        void StopListening(System.Action onChange);

//        T GetValue<T>();

//        void SetValue(object obj);

//        ObservableNodeTypes NodeType { get; }

//        IObservableNode Parent { get; }

//        IObservableNode GetChild(string name);

//        IEnumerable<IObservableNode> GetChildren();

//        IObservableNode GetChildFromPath(string path);

//        void SetListener(Action onTrigger);

//        void StopListener(Action onTrigger);
//    }

//    public class ObservableNode : IObservableNode
//    {
//        private Dictionary<string, IObservableNode> Children
//            = new Dictionary<string, IObservableNode>();

//        private bool _HasBeenSet = false;
//        private object _Value;
//        private System.Action OnChange = () => { };

//        public string Name { get; private set; }

//        public void Trigger()
//        {
//            this.OnChange();
//        }

//        public bool HasBeenSet
//        {
//            get
//            {
//                return this._HasBeenSet;
//            }
//        }

//        public void Listen(System.Action onChange)
//        {
//            this.OnChange = onChange;
//        }

//        public void StopListening(System.Action onChange)
//        {
//            this.OnChange -= onChange;
//        }

//        public T GetValue<T>()
//        {
//            return (T)this._Value;
//        }

//        public void SetValue(object obj)
//        {
//            this._HasBeenSet = true;

//            if (object.Equals(this._Value, obj))
//            {
//                return;
//            }

//            this._Value = obj;
//            this.Trigger();
//        }

//        public ObservableNodeTypes NodeType { get; private set; }

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
//            var node = new ObservableNode(name, this, ObservableNodeTypes.HierarchyOnly);
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

//        public void SetListener(Action onTrigger)
//        {
//            this.Listener += onTrigger;
//        }

//        public void StopListener(Action onTrigger)
//        {
//            this.Listener -= onTrigger;
//        }
//    }
//}