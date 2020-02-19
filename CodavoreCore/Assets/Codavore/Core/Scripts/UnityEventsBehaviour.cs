// <copyright file="UnityEventsBehaviour.cs" company="Codavore, LLC">
//     Copyright (c) Codavore, LLC. All rights reserved.
// </copyright>

namespace Codavore.Core
{
    using UnityEngine;

    /// <summary>
    /// Handles the Unity side of the events service.
    /// </summary>
    public class UnityEventsBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Holds a reference to the functions to call.
        /// </summary>
        private readonly IUnityEventsCaller caller;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityEventsBehaviour" /> class.
        /// </summary>
        public UnityEventsBehaviour()
        {
            this.caller = Locator.Get<IUnityEventsCaller>();
        }

        /// <summary>
        /// Transfers Unity's Awake function.
        /// </summary>
        private void Awake()
        {
            this.caller.Awake();
        }

        /// <summary>
        /// Transfers Unity's Start function.
        /// </summary>
        private void Start()
        {
            this.caller.Start();
        }

        /// <summary>
        /// Transfer's Unity's Stop function.
        /// </summary>
        private void Update()
        {
            this.caller.Update();
        }
    }
}
