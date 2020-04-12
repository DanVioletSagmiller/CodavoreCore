// <copyright file="Locator.cs" company="Codavore, LLC">
//     Copyright (c) Codavore, LLC. All rights reserved.
// </copyright>

namespace Codavore.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Locator provides references to service classes
    /// </summary>
    public static class Locator
    {
        /// <summary>
        /// Holds a reference to Locations;
        /// </summary>
        private static readonly Dictionary<System.Type, Location> Locations = new Dictionary<System.Type, Location>();

        /// <summary>
        /// Sets a Location, with an immediately bound reference.
        /// </summary>
        /// <typeparam name="I">The abstraction used as a key.</typeparam>
        /// <typeparam name="T">The concrete type to return.</typeparam>
        /// <param name="content">The concrete object to be returned on later calls.</param>
        public static void Set<I, T>(T content) where T : I
        {
            var key = typeof(I);
            if (Locations.ContainsKey(key))
            {
                throw new KeyNotUniqueException("A given key was already setup earlier in the code. Please be sure that this type is only set once.", key);
            }

            var location = new Location()
            {
                AbstractType = key,
                State = Location.LocationState.HasValue,
                Value = content
            };

            Locations.Add(key, location);
        }

        /// <summary>
        /// Sets a Location, with an immediately bound reference.
        /// </summary>
        /// <typeparam name="I">The abstraction used as a key.</typeparam>
        /// <typeparam name="T">The concrete type to return.</typeparam>
        public static void Set<I, T>() where T : I
        {
            Set<I>(() => { return (I)System.Activator.CreateInstance<T>(); });
        }

        /// <summary>
        /// Sets a Location, bound to another interface expected to be a subType.
        /// </summary>
        /// <typeparam name="I">The abstraction used as a key.</typeparam>
        /// <typeparam name="I2">The concrete type to return.</typeparam>
        public static void SetFrom<I, I2>()
        {
            // This assumes the base type inherits from both. If it is setup without a common base type, then it will error with a type cast exception.
            Set<I>(() => (I)(object)Locator.Get<I2>());
        }

        /// <summary>
        /// Sets a Location, with an immediately bound reference.
        /// </summary>
        /// <typeparam name="I">The abstraction used as a key.</typeparam>
        /// <param name="provider">A function to delay the binding.</param>
        public static void Set<I>(System.Func<I> provider)
        {
            var key = typeof(I);
            if (Locations.ContainsKey(key))
            {
                throw new KeyNotUniqueException("A given key was already setup earlier in the code. Please be sure that this type is only set once.", key);
            }

            var location = new Location()
            {
                AbstractType = key,
                State = Location.LocationState.HasConstructor,
                Create = new System.Func<object>(() => { return provider(); })
            };

            Locations.Add(key, location);
        }

        /// <summary>
        /// Retrieves an object based on a given type.
        /// </summary>
        /// <typeparam name="I">An abstracted type reference to the object.</typeparam>
        /// <returns>The concrete type for the given abstraction.</returns>
        public static I Get<I>()
        {
            var key = typeof(I);
            if (!Locations.ContainsKey(key))
            {
                throw new KeyNotFoundException(
                    "The given key of " +
                    key.FullName +
                    " could not be found. Make sure this class is added to the initializer class.");
            }

            var location = Locations[key];

            if (location.State == Location.LocationState.HasConstructor)
            {
                location.Value = location.Create();
                location.State = Location.LocationState.HasValue;
            }

            return (I)location.Value;
        }

        /// <summary>
        /// Provides a way to store references to locations.
        /// </summary>
        private class Location
        {
            /// <summary>
            /// Manages the different states of the <see cref="Location"/>.
            /// </summary>
            public enum LocationState
            {
                /// <summary>
                /// Default undefinted, if enum is generated
                /// </summary>
                UnDefined,

                /// <summary>
                /// The location is already storing the value.
                /// </summary>
                HasValue,

                /// <summary>
                /// The location must call the constructor to get the object.
                /// </summary>
                HasConstructor
            }

            /// <summary>
            /// Gets or sets the type used for abstracting the <see cref="Location"/>.
            /// </summary>
            public System.Type AbstractType { get; set; }

            /// <summary>
            /// Gets or sets the value assigned to the <see cref="Location"/>.
            /// </summary>
            public object Value { get; set; }

            /// <summary>
            /// Gets or sets the create function assigned to the <see cref="Location"/>.
            /// </summary>
            public System.Func<object> Create { get; set; }

            /// <summary>
            /// Gets or sets the state assigned to the <see cref="Location"/>.
            /// </summary>
            public LocationState State { get; set; }
        }
    }
}