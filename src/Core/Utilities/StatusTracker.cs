// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Timers;
    using Microsoft.Bing.Commerce.Connectors.Core.Serializers;
    using Microsoft.Bing.Commerce.Ingestion.Models;
    using NLog;

    internal class StatusTracker : IDisposable
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IngestionClient client;
        private readonly RequestLogger logger;
        private readonly string productIdFieldName;
        private readonly Timer timer;
        private readonly object timerLock = new object();

        private TimeSpan? trackingInterval;
        private List<UpdateTrackingRequest> updatesToTrack = new List<UpdateTrackingRequest>();

        public StatusTracker(IngestionClient client, TimeSpan? trackingInterval, RequestLogger logger)
        {
            Require.Instance.IsNotNull(logger, nameof(logger));
            this.trackingInterval = trackingInterval;
            if (this.trackingInterval != null && !logger.IsDeadletterLogEnabled())
            {
                log.Warn($"tracking cadence is set while deadletter log is disabled. Turning off status tracking.");
                this.trackingInterval = null;
            }

            if (this.IsValid())
            {
                Require.Instance.IsNotNull(client, nameof(client));
                this.client = client;
                this.logger = logger;
                this.productIdFieldName = this.FindProductIdFieldName();
                this.timer = new Timer(this.trackingInterval.Value.TotalMilliseconds);
                this.timer.AutoReset = true;
                this.timer.Elapsed += this.TimerTriggered;
            }
        }

        public void Add(string updateId, IEnumerable<IDictionary<string, object>> records)
        {
            if (this.IsValid())
            {
                this.updatesToTrack.Add(new UpdateTrackingRequest(updateId, records, this.productIdFieldName));
            }
        }

        public void Start()
        {
            if (this.IsValid())
            {
                this.timer.Start();
            }
        }

        public void Stop()
        {
            if (this.timer != null && this.timer.Enabled)
            {
                this.timer.Stop();
            }
        }

        public void Dispose()
        {
            this.timer.Dispose();
        }

        private void TimerTriggered(object sender, ElapsedEventArgs e)
        {
            lock (this.timerLock)
            {
                this.TrackUpdatesAsync().Wait();
            }
        }

        private async Task TrackUpdatesAsync()
        {
            List<IDictionary<string, object>> failedRecords = new List<IDictionary<string, object>>();
            HashSet<string> completedUpdates = new HashSet<string>();
            foreach (var request in this.updatesToTrack)
            {
                var statusResponse = await this.client.PushDataStatusAsync(request.UpdateId);
                if (statusResponse.Status == "InProgress")
                {
                    continue;
                }

                completedUpdates.Add(request.UpdateId);
                foreach (var record in statusResponse.Records)
                {
                    if (record.Status == "Failed")
                    {
                        request.Records[record.RecordId].Add("ERROR_MESSAGE", record.ErrorMessage);
                        failedRecords.Add(request.Records[record.RecordId]);
                    }
                }
            }

            this.updatesToTrack.RemoveAll((r) => completedUpdates.Contains(r.UpdateId));

            if (failedRecords.Count > 0)
            {
                this.logger.LogFailure(this.Serialize(failedRecords));
            }
        }

        private string FindProductIdFieldName()
        {
            var indexInfoResponse = this.client.GetIndexAsync().GetAwaiter().GetResult();
            Require.Instance.IsTrue(indexInfoResponse.Indexes.Count > 0, $"Index [{this.client.Indexid}] was not found, please ensure you provided the proper index id.");

            var indexInfo = indexInfoResponse.Indexes[0];

            return indexInfo.Fields.Where((f) => f.Type == IndexFieldType.ProductId).First().Name;
        }

        private string Serialize(List<IDictionary<string, object>> records)
        {
            return new JsonArraySerializer().Serialize(records);
        }

        private bool IsValid()
        {
            return this.trackingInterval?.TotalMilliseconds > 0;
        }

        internal class UpdateTrackingRequest
        {
            public UpdateTrackingRequest(string updateId, IEnumerable<IDictionary<string, object>> records, string updateIdFieldName)
            {
                this.Records = new Dictionary<string, IDictionary<string, object>>();

                foreach (var record in records)
                {
                    this.Records.Add(record[updateIdFieldName].ToString(), record);
                }

                this.UpdateId = updateId;
            }

            public Dictionary<string, IDictionary<string, object>> Records { get; set; }

            public string UpdateId { get; }
        }
    }
}
