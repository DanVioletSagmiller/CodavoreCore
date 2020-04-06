// <copyright file="ObservableRoot.cs" company="Codavore, LLC">
//     Copyright (c) Codavore, LLC. All rights reserved.
// </copyright>

namespace Newtonsoft.Json
{
    using System;

    public static class JsonTextReaderHelper
    {

        public static bool Validate(
            this JsonTextReader reader,
            JsonToken type,
            string complaint = "",
            string name = "",
            bool read = true,
            bool errorOnFailure = true)
        {
            if (read)
            {
                reader.Read();
            }
            if (reader.TokenType == JsonToken.PropertyName)
            {
                UnityEngine.Debug.Log(reader.LineNumber + ":" + reader.LinePosition + ":" + reader.TokenType + ":" + reader.Value.ToString());
            }
            else
            {
                UnityEngine.Debug.Log(reader.LineNumber + ":" + reader.LinePosition + ":" + reader.TokenType);
            }
            

            if (reader.TokenType != type)
            {
                if (!errorOnFailure)
                {
                    return false;
                }

                throw new FormatException(complaint + " - type did not match. Expected "
                    + type
                    + ", but received "
                    + reader.TokenType);
            }

            if (!string.IsNullOrWhiteSpace(name)
                && (string)reader.Value != name)
            {
                if (!errorOnFailure)
                {
                    return false;
                }

                throw new FormatException(complaint + " - Name did not match. Expected "
                    + name
                    + ", but received "
                    + (string)reader.Value);
            }

            return true;
        }
    }
}
