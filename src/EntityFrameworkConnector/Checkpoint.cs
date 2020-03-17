// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.EntityFramework
{
    using System.IO;
    using Microsoft.Bing.Commerce.Connectors.Core;

    class Checkpoint : IDataCheckpoint
    {
        private string currentCheckpoint = null;
        private readonly string checkpointFile;

        public Checkpoint(string checkpointFile)
        {
            this.checkpointFile = checkpointFile;
            var directory = Path.GetDirectoryName(checkpointFile);
            if (File.Exists(checkpointFile))
            {
                currentCheckpoint = File.ReadAllText(checkpointFile);
            }
            else if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(currentCheckpoint);
        }

        public string GetValue()
        {
            return currentCheckpoint?.ToString();
        }

        public void Accept(string newCheckpoint)
        {
            if (!string.IsNullOrEmpty(newCheckpoint))
            {
                currentCheckpoint = newCheckpoint;
                File.WriteAllText(checkpointFile, currentCheckpoint.ToString());
            }
        }
    }
}
