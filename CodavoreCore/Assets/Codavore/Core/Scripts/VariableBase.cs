// <copyright file="VariableBase.cs" company="Codavore, LLC">
//     Copyright (c) Codavore, LLC. All rights reserved.
// </copyright>

namespace Codavore.Core
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public interface IVariableBase<T>
    {
        void Listen(Action<T> listener, object source);
        object[] ListeningSources { get; }
        void Reset();
        void StopListening(Action<T> listener);
        void Trigger(T value);
        T Value { get; set; }
    }

    /// <summary>
    /// Service for logging and analytics.
    /// </summary>
    public class VariableBase<T> : ScriptableObject, IVariableBase<T>
    {
        private Action<T> _Listener = (t) => { };

        [SerializeField]
        private T _Value;

        [SerializeField]
        private T DefaultValue;

        private List<object> _ListenerObjects = new List<object>();

        private void Awake()
        {
            this.Reset();
        }

        public void Reset()
        {
            this._Value = DefaultValue;
            Trigger(this._Value);
        }
        `
        public T Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if (_Value == null && value == null)
                {
                    return;
                }

                if (_Value != null)
                {
                    if (_Value.Equals(value))
                    {
                        return;
                    }
                    ChangeValue(value);
                }
            }
        }

        public object[] ListeningSources
        {
            get
            {
                return this._ListenerObjects.ToArray();
            }
        }

        private void ChangeValue(T value)
        {
            _Value = value;
            _Listener(value);
        }

        public void Listen(Action<T> listener, object source)
        {
            _Listener += listener;
        }

        public void StopListening(Action<T> listener)
        {
            _Listener += listener;
        }

        public void Trigger(T value)
        {
            _Value = value;
            this._Listener(value);
        }
    }
}
