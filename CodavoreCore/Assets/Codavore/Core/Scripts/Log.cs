// <copyright file="Log.cs" company="Codavore, LLC">
//     Copyright (c) Codavore, LLC. All rights reserved.
// </copyright>

namespace Codavore.Core
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Stores and reports messages for debugging and analytics
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Logs information.
        /// </summary>
        /// <param name="path">Where the issue happened.</param>
        /// <param name="message">The information.</param>
        void Info(string path, string message);

        /// <summary>
        /// Logs a warning.
        /// </summary>        
        /// <param name="path">Where the issue happened.</param>
        /// <param name="message">The information.</param>
        void Warning(string path, string message);

        /// <summary>
        /// Logs an Exception.
        /// </summary>
        /// <param name="path">Where the issue happened.</param>
        /// <param name="message">The information.</param>
        /// <param name="exception">The exception.</param>
        void Exception(string path, string message, System.Exception exception);

        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="path">Where the issue happened.</param>
        /// <param name="message">The information.</param>
        void Error(string path, string message);
    }

    /// <summary>
    /// Service for logging and analytics.
    /// </summary>
    public class Log : ILog
    {
        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="path">Where the issue happened.</param>
        /// <param name="message">The information.</param>
        public void Error(string path, string message)
        {
            string log = this.WrapColor(Color.red, path) + Environment.NewLine + message;
            UnityEngine.Debug.LogError(log);
        }

        /// <summary>
        /// Logs an Exception.
        /// </summary>
        /// <param name="path">Where the issue happened.</param>
        /// <param name="message">The information.</param>
        /// <param name="exception">The exception.</param>
        public void Exception(string path, string message, Exception exception)
        {
            string log = 
                this.WrapColor(Color.red, path) + 
                Environment.NewLine + 
                message +
                Environment.NewLine +
                exception.ToString();

            UnityEngine.Debug.LogError(log);
        }

        /// <summary>
        /// Logs information.
        /// </summary>
        /// <param name="path">Where the issue happened.</param>
        /// <param name="message">The information.</param>
        public void Info(string path, string message)
        {
            string log = this.WrapColor(Color.green, path) + Environment.NewLine + message;
            UnityEngine.Debug.Log(log);
        }

        /// <summary>
        /// Logs a warning.
        /// </summary>        
        /// <param name="path">Where the issue happened.</param>
        /// <param name="message">The information.</param>
        public void Warning(string path, string message)
        {
            string log = this.WrapColor(new Color(1f, 165f / 255f, 0f), path) + 
                Environment.NewLine + 
                message;

            UnityEngine.Debug.LogError(log);
        }

        /// <summary>
        /// Surrounds text with a Unity formatting code for adding color.
        /// </summary>
        /// <param name="color">The color to set the content to.</param>
        /// <param name="content">The text to have its color set.</param>
        /// <returns>Unity color formatted text.</returns>
        private string WrapColor(UnityEngine.Color color, string content)
        {
            return 
                "<color=#" + 
                UnityEngine.ColorUtility.ToHtmlStringRGBA(color) + 
                ">" + 
                content + 
                "</color>";
        }
    }
}
