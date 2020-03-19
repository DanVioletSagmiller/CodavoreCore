// <copyright file="VariableBase.cs" company="Codavore, LLC">
//     Copyright (c) Codavore, LLC. All rights reserved.
// </copyright>

namespace Codavore.Core
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public interface IObservableNode
    {
        T GetValue<T>();
        
        void SetValue(object value);
        
        void StartListening(Action listener);
        
        void StopListening(Action listener);
        
        IObservableNode GetParent();

        string GetName();

        IObservableNode GetChild(string name);

        IObservableNode GetChildFromPath(string path);
    }

    public class ObservableNode : IObservableNode
    {
        private static readonly string PathSeparaterString = "/";

        private static readonly char[] PathSeparaterCharArray = { PathSeparaterString[0] };

        private object Value;

        private Action OnValueChange = () => { };

        private IObservableNode Parent = null;

        private string Name = "";

        private Dictionary<string, IObservableNode> Children
            = new Dictionary<string, IObservableNode>();

        public ObservableNode(string name, IObservableNode parent)
        {
            this.Name = name;
            this.Parent = parent;
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

            var isNotPath = !path.Contains(PathSeparaterString);
            if (isNotPath)
            {
                return GetChild(path);
            }

            var segments = path.Split(PathSeparaterCharArray);
            IObservableNode node = this;
            foreach(var segment in segments)
            {
                node = node.GetChild(segment);
            }

            return node;
        }
    }
}
