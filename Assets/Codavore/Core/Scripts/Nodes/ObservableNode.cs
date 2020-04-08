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

        string GetPath();
        
        void SetValue(object value);
        
        void StartListening(Action listener);
        
        void StopListening(Action listener);

        string GetName();

        void Reset(string name, string path, object value);
    }

    public class ObservableNode : IObservableNode
    {
        private Guid Guid;
        private string Name;
        private object Value;
        private string Path;
        private Action OnChange = () => { };

        public ObservableNode(Guid guid)
        {
            this.Guid = guid;
        }

        public Guid GetGuid()
        {
            return this.Guid;
        }

        public string GetName()
        {
            return this.Name;
        }

        public string GetPath()
        {
            return this.Path;
        }

        public T GetValue<T>()
        {
            return (T)this.Value;
        }

        public void Reset(string name, string path, object value)
        {
            this.Name = name;
            this.Path = path;
            this.SetValue(value);
        }

        public void SetValue(object value)


        {
            if (value == null && this.Value == null)
            {
                return;
            }

            if (value != null && this.Value == null)
            {
                this.Value = value;
                this.OnChange();
                return;
            }

            if (value == null && this.Value != null)
            {
                this.Value = null;
                this.OnChange();
                return;
            }

            if (this.Value.Equals(value))
            {
                return;
            }

            this.Value = value;
            this.OnChange();
        }

        public void StartListening(Action listener)
        {
            this.OnChange += listener;
        }

        public void StopListening(Action listener)
        {
            this.OnChange -= listener;
        }
    }
}
