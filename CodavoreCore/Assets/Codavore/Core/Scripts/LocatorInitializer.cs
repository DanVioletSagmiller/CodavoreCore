// <copyright file="LocatorInitializer.cs" company="Codavore, LLC">
//     Copyright (c) Codavore, LLC. All rights reserved.
// </copyright>

namespace Codavore.Core
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Handles the initialization for Locator.
    /// </summary>
    public static class LocatorInitializer
    {
        /// <summary>
        /// Defines if this class has already been initialized before scene.
        /// </summary>
        private static bool initialized = false;

        /// <summary>
        /// Holds behavior information to validate after scene load.
        /// </summary>
        private static List<Type> behaviourChecks = new List<Type>();

        /// <summary>
        /// Triggers the initialization, prior to the scene load.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private static void Init()
        {
            if (initialized)
            {
                return;
            }

            initialized = true;

            MapTypes();
            SetupMonoVerifications();
        }

        /// <summary>
        /// Establishes which MonoBehaviours to validate.
        /// </summary>
        private static void SetupMonoVerifications()
        {
            behaviourChecks.Add(typeof(UnityEventsBehaviour));
        }

        /// <summary>
        /// Establishes the methods.
        /// </summary>
        private static void MapTypes()
        {
            Locator.Set<ILog, Log>();
            Locator.Set<IUnityEvents, UnityEvents>();

            Locator.SetFrom<IUnityEventsCaller, IUnityEvents>();
        }

        /// <summary>
        /// This validated the behaviours have been added to the scene.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void VerifyBehaviours()
        {
            var log = Locator.Get<ILog>();
            var path = typeof(LocatorInitializer).FullName + "." + nameof(VerifyBehaviours);
            foreach (var type in behaviourChecks)
            {
                if (GameObject.FindObjectOfType(type) == null)
                {
                    var errMsg = type.FullName + " should be attached to a game object as part of management services, but it was not found.";
                    log.Error(path, errMsg);
                }
            }
        }
    }
}