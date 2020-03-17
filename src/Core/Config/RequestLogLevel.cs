// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.Core.Config
{
    /// <summary>
    /// The requested level for logging full requests.
    /// </summary>
    public enum RequestLogLevel
    {
        /// <summary>
        /// No request logging is requested.
        /// </summary>
        None,

        /// <summary>
        /// Only log failed requests / records.
        /// </summary>
        DeadletterOnly,

        /// <summary>
        /// Log everything.
        /// </summary>
        All
    }
}
