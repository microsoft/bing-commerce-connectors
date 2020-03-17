// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.Core.Serializers
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface to describe how to serialize the push batch before sending to the Bing for Commerce ingestion API.
    /// </summary>
    /// <typeparam name="T">The type being serialized.</typeparam>
    public interface IPushSerializer<T>
    {
        /// <summary>
        /// Gets the number of characters added to any number of serialized records in order to complete the batch. For example, in JSonArray, this would be 2 (for the angle brackets).
        /// </summary>
        uint OverheadSize { get; }

        /// <summary>
        /// Gets the number of characters added as an overhaed to one serialized record in order to be able to concatenate it with other serialized records. For example, in JSonArray, this would be 1 (for the coomma).
        /// </summary>
        uint RecordOverheadSize { get; }

        /// <summary>
        /// Serializes a batch of record objects.
        /// </summary>
        /// <param name="records">the records to be serialized.</param>
        /// <returns>The batch of objects serialized as string.</returns>
        string Serialize(IEnumerable<T> records);

        /// <summary>
        /// Serializes a single record object.
        /// </summary>
        /// <param name="record">The record object to be serialized</param>
        /// <returns>The object serialized as string.</returns>
        string Serialize(T record);
    }
}
