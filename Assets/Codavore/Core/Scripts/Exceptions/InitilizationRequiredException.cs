// <copyright file="InitilizationRequiredException.cs" company="Codavore, LLC">
//     Copyright (c) Codavore, LLC. All rights reserved.
// </copyright>

namespace Codavore.Core
{
    using System;

    /// <summary>
    /// An Exception thrown when a key needs to be unique, but is not.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "<Pending>")]
    public class InitilizationRequiredException : Exception
    {
        /// <summary>
        /// Gets or sets the name of an event.
        /// </summary>
        public string MethodName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InitilizationRequiredException" /> class.
        /// </summary>
        /// <param name="message">The message explaining what went wrong, and suggestions to fix it.</param>
        public InitilizationRequiredException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InitilizationRequiredException" /> class.
        /// </summary>
        /// <param name="message">The message explaining what went wrong, and suggestions to fix it.</param>
        /// <param name="innterException">A lower level exception with more specific detail into the problem.</param>
        public InitilizationRequiredException(string message, Exception innterException) : base(message, innterException)
        {
        }
    }
}
