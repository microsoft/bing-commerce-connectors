// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.Core.Serializers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;

    /// <summary>
    /// A utility to serialize the batch of records in New-line Delimited Json format.
    /// </summary>
    public class NDJsonSerializer : IPushSerializer<IDictionary<string, object>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NDJsonSerializer"/> class.
        /// </summary>
        public NDJsonSerializer()
        {
        }

        /// <summary>
        /// Gets the number of characters added to any number of serialized records in order to complete the batch. That is zero for NDJson Serialization.
        /// </summary>
        public uint OverheadSize => 0;

        /// <summary>
        /// Gets the number of characters added as an overhaed to one serialized record in order to be able to concatenate it with other serialized records. That is the new line characters for NDJson Serialization
        /// </summary>
        public uint RecordOverheadSize => (uint)Environment.NewLine.Length;

        /// <summary>
        /// Gets or sets the optional serialization settings to use when serializing the json objects.
        /// </summary>
        public JsonSerializerSettings SerializationSettings { get; set; }

        /// <summary>
        /// Serializes a batch of record objects in New-line Delimited Json format.
        /// </summary>
        /// <param name="records">the records to be serialized.</param>
        /// <returns>The batch of objects serialized as string.</returns>
        public string Serialize(IEnumerable<IDictionary<string, object>> records)
        {
            var serializer = JsonSerializer.Create(this.SerializationSettings);

            using (var sw = new StringWriter())
            using (var writer = new JsonTextWriter(sw))
            {
                bool first = true;

                foreach (var record in records)
                {
                    if (!first)
                    {
                        sw.WriteLine();
                    }

                    first = false;
                    serializer.Serialize(writer, record);
                }

                return sw.ToString();
            }
        }

        /// <summary>
        /// Serializes a single record object in New-line Delimited Json format.
        /// </summary>
        /// <param name="record">The record object to be serialized</param>
        /// <returns>The object serialized as string in Csv format.</returns>
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
