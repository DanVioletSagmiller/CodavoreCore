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

//        void StartListener(Action onTrigger);

//        void StopListener(Action onTrigger);
//    }

//    public class ObservableNode : IObservableNode
//    {
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