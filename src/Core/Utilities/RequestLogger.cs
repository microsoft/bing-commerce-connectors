// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.Core.Utilities
{
    using System;
    using System.IO;
    using Microsoft.Bing.Commerce.Connectors.Core.Config;

    internal class RequestLogger
    {
        private readonly DirectoryInfo deadletterLocation;
        private readonly DirectoryInfo successfulLocation;

        public RequestLogger(string logLocation, RequestLogLevel level)
        {
            Require.Instance.IsTrue(logLocation != null || level == RequestLogLevel.None, "Can't have an null directory path when log level is not none.    ");

            if (level != RequestLogLevel.None)
            {
                this.deadletterLocation = Directory.CreateDirectory(Path.Combine(logLocation, "deadletter"));
            }

            if (level == RequestLogLevel.All)
            {
                this.successfulLocation = Directory.CreateDirectory(Path.Combine(logLocation, "successful"));
            }
        }

        public void LogSuccess(string request)
        {
            if (this.IsSuccessLogEnabled())
            {
                File.WriteAllTextAsync(Path.Combine(this.successfulLocation.FullName, Guid.NewGuid().ToString()), request);
            }
        }

        public void LogFailure(string request)
        {
            if (this.IsDeadletterLogEnabled())
            {
                File.WriteAllTextAsync(Path.Combine(this.deadletterLocation.FullName, Guid.NewGuid().ToString()), request);
            }
        }

        public bool IsSuccessLogEnabled()
        {
            return this.successfulLocation != null;
        }

        public bool IsDeadletterLogEnabled()
        {
            return this.deadletterLocation != null;
        }
    }
}
