// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.Core.Utilities
{
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Bing.Commerce.Connectors.Core.Serializers;

    internal class RequestList<TRequest>
    {
        private readonly uint maximumRecordCount;
        private readonly uint maxRequestSize;
        private readonly IPushSerializer<TRequest> serializer;
        private List<TRequest> records;
        private uint currentSize;

        public RequestList(uint maximumRecordCount, uint maxRequestSize, IPushSerializer<TRequest> serializer)
        {
            this.maximumRecordCount = maximumRecordCount;
            this.maxRequestSize = maxRequestSize;
            this.serializer = serializer;
            this.ResetList();
        }

        public List<TRequest> Add(TRequest newRecord)
        {
            List<TRequest> result = this.CanAdd(newRecord);
            this.records.Add(newRecord);

            if (result == null && this.records.Count >= this.maximumRecordCount)
            {
                return this.ResetList();
            }

            return result;
        }

        public List<TRequest> ResetList()
        {
            var local = this.records;
            this.records = new List<TRequest>();
            this.currentSize = this.serializer.OverheadSize;

            return local;
        }

        private List<TRequest> CanAdd(TRequest newRecord)
        {
            if (this.records.Count >= this.maximumRecordCount || !this.ValidateCurrentSize(newRecord))
            {
                return this.ResetList();
            }

            return null;
        }

        private bool ValidateCurrentSize(TRequest record)
        {
            if (this.maxRequestSize == 0 || record == null)
            {
                return true;
            }

            this.currentSize += (uint)Encoding.UTF8.GetByteCount(this.serializer.Serialize(record));
            this.currentSize += this.serializer.RecordOverheadSize;

            return this.currentSize < this.maxRequestSize;
        }
    }
}
