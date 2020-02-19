// <copyright file="KeyNotUniqueException.cs" company="Codavore, LLC">
//     Copyright (c) Codavore, LLC. All rights reserved.
// </copyright>

namespace Codavore.Core
{
    using System;

    /// <summary>
    /// An Exception thrown when a key needs to be unique, but is not.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "<Pending>")]
    public class KeyNotUniqueException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyNotUniqueException" /> class.
        /// </summary>
        /// <param name="message">The message explaining what went wrong, and suggestions to fix it.</param>
        /// <param name="key">A reference to the specific object acting as the key.</param>
        public KeyNotUniqueException(string message, object key) : base(message)
        {
            this.Key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyNotUniqueException" /> class.
        /// </summary>
        /// <param name="message">The message explaining what went wrong, and suggestions to fix it.</param>
        /// <param name="innterException">A lower level exception with more specific detail into the problem.</param>
        /// <param name="key">A reference to the specific object acting as the key.</param>
        public KeyNotUniqueException(string message, Exception innterException, object key) : base(message, innterException)
        {
            this.Key = key;
        }

        /// <summary>
        /// Gets or sets a reference to the specific key.
        /// </summary>
        public object Key { get; set; }
    }
}
