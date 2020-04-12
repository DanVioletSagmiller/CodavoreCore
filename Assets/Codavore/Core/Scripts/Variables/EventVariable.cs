// <copyright file="VariableBase.cs" company="Codavore, LLC">
//     Copyright (c) Codavore, LLC. All rights reserved.
// </copyright>

namespace Codavore.Core
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Service for logging and analytics.
    /// </summary>
    [CreateAssetMenu(menuName = "Codavore/Variables/Event")]
    public class EventVariable : ScriptableObject
    {
        private Action _Listener = () => { };

        public Action Listen(Action listener)
        {
            _Listener += listener;
            return listener;
        }

        public void StopListening(Action listener)
        {
            _Listener -= listener;
        }

        public void Trigger()
        {
            _Listener();
        }
    }
}
