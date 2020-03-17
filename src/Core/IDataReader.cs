// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// An interface that the connector can use to poll the data from the data source.
    /// </summary>
    public interface IDataReader
    {
        /// <summary>
        /// Read the next batch of data from the data source, after the given checkpoint.
        /// </summary>
        /// <param name="checkpoint">The checkpoint to read the data since if valid.</param>
        /// <returns>Lists the new data since the given checkpoint.</returns>
        IEnumerable<DataPoint> ReadNext(IDataCheckpoint checkpoint);
    }
}
