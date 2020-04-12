// <copyright file="ObservableStorage.cs" company="Codavore, LLC">
//     Copyright (c) Codavore, LLC. All rights reserved.
// </copyright>

namespace Codavore.Core
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEngine;

    public interface IObservableStorage
    {
        void Load(string nodePath);

        void ForceLoad(string nodePath);
        string Save(string nodePath);
    }

    public class ObservableStorage : IObservableStorage
    {
        private IObservableRoot Root = Locator.Get<IObservableRoot>();
        private ILog Log = Locator.Get<ILog>();
        private string SavePath = Application.persistentDataPath;
        private List<string> LoadedPaths = new List<string>();
        private readonly string Extension = ".ors"; // ORS => Observable Root Storage

        public void Load(string nodePath)
        {
            if (LoadedPaths.Contains(nodePath)) return;

            ForceLoad(nodePath);
        }

        public void ForceLoad(string nodePath)
        {
            var logPath = nameof(ObservableStorage) + "." + nameof(ForceLoad);
            var localPath = GetLocalPathFor(nodePath);
            if (!File.Exists(localPath))
            {
                this.Log.Warning(logPath, "Attempted to load the file " + nodePath + this.Extension + ", but it did not exist.");
                return;
            }
            var json = System.IO.File.ReadAllText(localPath);
        }

        private string GetLocalPathFor(string nodePath)
        {
            if (nodePath == null)
            {
                throw new ArgumentNullException(nodePath, 
                    "Path cannot be null when using any function in IObservableStorage. If you intended to get the root folder, please use an empty string instead.");
            }

            var correctedPath = nodePath.Replace(CodavoreConstants.PathSeparaterChar, System.IO.Path.DirectorySeparatorChar);
            var fullPath = this.SavePath + CodavoreConstants.PathSeparaterString + correctedPath + this.Extension;
            return fullPath;
        }

        public string Save(string nodePath)
        {
            var path = this.GetLocalPathFor(nodePath);
            var contents = this.Root.SaveLineage(nodePath);
            var directory = Path.GetDirectoryName(path);
            Directory.CreateDirectory(directory);
            File.WriteAllText(path, contents);

            return contents;
        }
    }
}
