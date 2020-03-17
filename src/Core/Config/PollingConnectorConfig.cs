// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.Core.Config
{
    using System;

    /// <summary>
    /// The configurations needed for the polling connector.
    /// </summary>
    public class PollingConnectorConfig
    {
        /// <summary>
        /// Gets or sets the interval to use when polling the data.
        /// </summary>
        public TimeSpan ScanInterval { get; set; }

        /// <summary>
        /// Validates the configurations are within proper boundaries.
        /// </summary>
        /// <returns>this</returns>
        public PollingConnectorConfig Check()
        {
            return this;
        }
    }
}
