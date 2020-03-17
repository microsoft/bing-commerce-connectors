// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Describes the data record that the data source returns to the connector.
    /// </summary>
    public class DataPoint
    {
        /// <summary>
        /// Gets or sets a dicionary describing the record to be pushed.
        /// </summary>
        public IDictionary<string, object> Record { get; set; }

        /// <summary>
        /// Gets or sets the intended operaiton type to do with the current record.
        /// </summary>
        public DataOperation OperationType { get; set; }

        /// <summary>
        /// Gets or sets the current record checkpoint value.
        /// </summary>
        public string Checkpoint { get; set; }
    }
}
