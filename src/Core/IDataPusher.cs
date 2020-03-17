// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.Core
{
    /// <summary>
    /// An interface to describe the strategy that's used to push the data.
    /// </summary>
    public interface IDataPusher
    {
        /// <summary>
        /// Use the given data source to push data to the Bing for Commerce endpoint.
        /// </summary>
        /// <param name="data">the data source to use to poll the data from.</param>
        void Push(IDataReader data);
    }
}
