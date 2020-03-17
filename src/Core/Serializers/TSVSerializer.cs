// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.Core.Serializers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A utility to serialize the batch of records in Tab-Separated Values format.
    /// </summary>
    public class TSVSerializer : IPushSerializer<IDictionary<string, object>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TSVSerializer"/> class.
        /// </summary>
        public TSVSerializer()
        {
        }

        /// <summary>
        /// Gets the number of characters added to any number of serialized records in order to complete the batch. That is zero for TSV Serialization.
        /// </summary>
        public uint OverheadSize => 0;

        /// <summary>
        /// Gets the number of characters added as an overhaed to one serialized record in order to be able to concatenate it with other serialized records. That is the new line characters for TSV Serialization
        /// </summary>
        public uint RecordOverheadSize => (uint)Environment.NewLine.Length;

        /// <summary>
        /// Serializes a batch of record objects in Tab-Separated Values format.
        /// </summary>
        /// <param name="records">the records to be serialized.</param>
        /// <returns>The batch of objects serialized as string.</returns>
        public string Serialize(IEnumerable<IDictionary<string, object>> records)
        {
            var serializedRecords = records.Select(
                r => this.Serialize(r));

            return string.Join(Environment.NewLine, serializedRecords);
        }

        /// <summary>
        /// Serializes a single record object in Tab-Separated Values format.
        /// </summary>
        /// <param name="record">The record object to be serialized</param>
        /// <returns>The object serialized as string in Csv format.</returns>
        public string Serialize(IDictionary<string, object> record)
        {
            return string.Join('\t', record.Values.Select((o) => o.ToString().Replace("\t", "\\t").Replace("\n", "\\n").Replace("\r", "\\r")));
        }
    }
}
