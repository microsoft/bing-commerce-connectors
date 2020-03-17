// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.Core.Serializers
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// A utility to serialize the batch of records in JSon Array format.
    /// </summary>
    public class JsonArraySerializer : IPushSerializer<IDictionary<string, object>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonArraySerializer"/> class.
        /// </summary>
        public JsonArraySerializer()
        {
        }

        /// <summary>
        /// Gets the number of characters added to any number of serialized records in order to complete the batch. That is the sqauare brackets for the Json Array.
        /// </summary>
        public uint OverheadSize => 2;

        /// <summary>
        /// Gets the number of characters added as an overhaed to one serialized record in order to be able to concatenate it with other serialized records. That is the comma for Json Array Serialization.
        /// </summary>
        public uint RecordOverheadSize => 1;

        /// <summary>
        /// Gets or sets the optional serialization settings to use when serializing the json objects.
        /// </summary>
        public JsonSerializerSettings SerializationSettings { get; set; }

        /// <summary>
        /// Serializes a batch of record objects in Json Array format.
        /// </summary>
        /// <param name="records">the records to be serialized.</param>
        /// <returns>The batch of objects serialized as string.</returns>
        public string Serialize(IEnumerable<IDictionary<string, object>> records)
        {
            return this.SerializeInternal(records);
        }

        /// <summary>
        /// Serializes a single record object in Json Array format.
        /// </summary>
        /// <param name="record">The record object to be serialized</param>
        /// <returns>The object serialized as string.</returns>
        public string Serialize(IDictionary<string, object> record)
        {
            return this.SerializeInternal(record);
        }

        private string SerializeInternal<T>(T obj)
        {
            return this.SerializationSettings == null ? JsonConvert.SerializeObject(obj) : JsonConvert.SerializeObject(obj, this.SerializationSettings);
        }
    }
}
