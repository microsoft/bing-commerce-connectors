// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.Core.Serializers
{
    using System.Collections.Generic;
    using Microsoft.Bing.Commerce.Connectors.Core.Config;

    /// <summary>
    /// A utility to serialize the batch of records in the one of select format.
    /// </summary>
    public class FormatSerializer : IPushSerializer<IDictionary<string, object>>
    {
        private static readonly Dictionary<Format, IPushSerializer<IDictionary<string, object>>> serializerMap =
            new Dictionary<Format, IPushSerializer<IDictionary<string, object>>>()
            {
                { Format.JsonArray, new JsonArraySerializer() },
                { Format.NDJson, new NDJsonSerializer() },
                { Format.Csv, new CSVSerializer() },
                { Format.Tsv, new TSVSerializer() }
            };

        private readonly IPushSerializer<IDictionary<string, object>> activeSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatSerializer"/> class.
        /// </summary>
        /// <param name="serializerFormat">The selected format to intialize the object with.</param>
        public FormatSerializer(Format serializerFormat)
        {
            this.activeSerializer = serializerMap[serializerFormat];
        }

        /// <summary>
        /// Gets the number of characters added to any number of serialized records in order to complete the batch.
        /// </summary>
        public uint OverheadSize => this.activeSerializer.OverheadSize;

        /// <summary>
        ///  Gets the number of characters added as an overhaed to one serialized record in order to be able to concatenate it with other serialized records.
        /// </summary>
        public uint RecordOverheadSize => this.activeSerializer.RecordOverheadSize;

        /// <summary>
        /// Uses the selected serializer to serialize a batch of record objects in Comma-Separated Values format.
        /// </summary>
        /// <param name="records">the records to be serialized.</param>
        /// <returns>The batch of objects serialized as string.</returns>
        public string Serialize(IEnumerable<IDictionary<string, object>> records)
        {
            return this.activeSerializer.Serialize(records);
        }

        /// <summary>
        /// uses the selectd serializer to serializes a single record object in Comma-Separated Values format.
        /// </summary>
        /// <param name="record">The record object to be serialized</param>
        /// <returns>The object serialized as string in Csv format.</returns>
        public string Serialize(IDictionary<string, object> record)
        {
            return this.activeSerializer.Serialize(record);
        }
    }
}
