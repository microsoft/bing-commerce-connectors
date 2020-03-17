// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Bing.Commerce.Connectors.Core.Config;
    using Microsoft.Bing.Commerce.Connectors.Core.Serializers;
    using Microsoft.Bing.Commerce.Connectors.Core.Utilities;
    using Microsoft.Bing.Commerce.Ingestion;
    using Microsoft.Rest;
    using NLog;

    /// <summary>
    /// A simple Bing for Commerce pusher, that can support capping the concurrent push requests.
    /// </summary>
    public class SimpleBingCommercePusher : IDataPusher, IDisposable
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly BingCommerceConfig config;
        private readonly TaskManager taskManager;
        private readonly StatusTracker tracker;
        private readonly IngestionClient client;
        private readonly CheckpointAcceptor checkpointAcceptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleBingCommercePusher"/> class.
        /// </summary>
        /// <param name="config">The pusher configurations object</param>
        /// <param name="checkpoint">The checkpoint to poll the data since if it's valid.</param>
        /// <param name="serializer">(Optional): Explicit serialier to be used.</param>
        public SimpleBingCommercePusher(BingCommerceConfig config, IDataCheckpoint checkpoint, IPushSerializer<IDictionary<string, object>> serializer = null)
        {
            var sdkClient = new BingCommerceIngestion(new TokenCredentials(config.AccessToken));
            var logger = new RequestLogger(config.RequestLogLocation, config.RequestLog);

            this.config = config;
            this.client = new IngestionClient(sdkClient, config.TenantId, config.IndexId, config.RetryCount, logger);
            this.tracker = new StatusTracker(this.client, config.TrackingInterval, logger);
            this.taskManager = new TaskManager(config.MaxConcurrentRequests);
            this.checkpointAcceptor = new CheckpointAcceptor(checkpoint);
            this.Serializer = serializer ?? new FormatSerializer(config.PushFormat);

            log.Debug("Successfully created the Simple Bing Commerce Pusher with the provided access token.");
        }

        /// <summary>
        /// Gets or sets the serializer used to serialize the given records into string.
        /// </summary>
        public IPushSerializer<IDictionary<string, object>> Serializer { get; set; }

        /// <summary>
        /// Use the given data source to push the data after the given checkpoint to the Bing for Commerce endpoint.
        /// </summary>
        /// <param name="data">the data source to use to poll the data from.</param>
        public void Push(IDataReader data)
        {
            log.Debug($"Starting a data push, starting from the checkpoint: {this.checkpointAcceptor.Checkpoint.GetValue()}.");

            this.tracker.Start();

            var requestList = new RequestList<IDictionary<string, object>>(this.config.MaxBatchCount, this.config.MaxRequestSize, this.Serializer);
            string latestCheckpoint = null;

            foreach (var record in data.ReadNext(this.checkpointAcceptor.Checkpoint))
            {
                if (record.OperationType == DataOperation.Update)
                {
                    var updates = requestList.Add(record.Record);
                    if (updates != null)
                    {
                        string localCheckpoint = latestCheckpoint;
                        this.checkpointAcceptor.Pending(localCheckpoint);
                        this.taskManager.Add(() => this.SendRequestAsync(updates, () => this.checkpointAcceptor.Accept(localCheckpoint)));
                    }

                    latestCheckpoint = record.Checkpoint;
                }
                else
                {
                    throw new NotSupportedException($"Operation is not supported: {record.OperationType}");
                }
            }

            var updatedRecords = requestList.ResetList();
            if (updatedRecords.Count > 0)
            {
                this.checkpointAcceptor.Pending(latestCheckpoint);
                this.taskManager.Add(() => this.SendRequestAsync(updatedRecords, () => this.checkpointAcceptor.Accept(latestCheckpoint)));
            }

            log.Debug($"Ending a data push. Current checkpoint: {this.checkpointAcceptor.Checkpoint.GetValue()}.");
        }

        /// <summary>
        /// Dispose the object's resources.
        /// </summary>
        public void Dispose()
        {
            this.client.Dispose();
            this.tracker.Dispose();
        }

        private async Task SendRequestAsync(List<IDictionary<string, object>> updatedRecords, Action onSuccess)
        {
            log.Info($"Sending Request to Bing Commerce Ingestion service. Current request size: {updatedRecords.Count}.");
            var request = this.Serialize(updatedRecords);
            try
            {
                var response = await this.client.PushDataUpdateAsync(request);

                if (response != null)
                {
                    log.Debug($"Push Data Update request successful. Update Id: {response.UpdateId}");
                    this.tracker.Add(response.UpdateId, updatedRecords);
                }

                onSuccess();
            }
            catch (Exception e)
            {
                log.Warn(e, $"Failed to push the data to bing commerce ingestion apis. Exception: {e.Message}");
            }
        }

        private string Serialize(List<IDictionary<string, object>> records)
        {
            log.Debug($"Serializing [{records.Count}] records with the [{this.config.PushFormat.ToString()}] format.");
            return this.Serializer.Serialize(records);
        }
    }
}
