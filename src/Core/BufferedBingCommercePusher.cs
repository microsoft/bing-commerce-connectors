// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Bing.Commerce.Connectors.Core.Config;
    using Microsoft.Bing.Commerce.Connectors.Core.Serializers;
    using Microsoft.Bing.Commerce.Connectors.Core.Utilities;
    using Microsoft.Bing.Commerce.Ingestion;
    using Microsoft.Rest;
    using NLog;

    /// <summary>
    /// A Bing for Commerce pusher that supports buffering updates for a configurable time period before sending all buffered records, or when it reached the configurable maximum records per request.
    /// </summary>
    public class BufferedBingCommercePusher : IDisposable
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly BingCommerceConfig config;
        private readonly IPushSerializer<IDictionary<string, object>> serializer;
        private readonly IngestionClient client;
        private readonly BufferedSender<IDictionary<string, object>> updateSender;
        private readonly StatusTracker statusTracker;
        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedBingCommercePusher"/> class.
        /// </summary>
        /// <param name="config">The pusher configuration.</param>
        public BufferedBingCommercePusher(BingCommerceConfig config)
        {
            Require.Instance.IsNotNull(config, nameof(config));

            this.config = config;
            var sdkclient = new BingCommerceIngestion(new TokenCredentials(config.AccessToken));
            var logger = new RequestLogger(config.RequestLogLocation, config.RequestLog);
            this.client = new IngestionClient(sdkclient, config.TenantId, config.IndexId, config.RetryCount, logger);

            this.statusTracker = new StatusTracker(this.client, config.TrackingInterval, logger);

            this.serializer = new FormatSerializer(config.PushFormat);

            log.Debug("Successfully created the Simple Bing Commerce Pusher with the provided access token.");

            this.updateSender = new BufferedSender<IDictionary<string, object>>(
                config.MaxBufferWaitMs,
                new TaskManager(config.MaxConcurrentRequests),
                new RequestList<IDictionary<string, object>>(config.MaxBatchCount, config.MaxRequestSize, this.serializer),
                (records) => this.SendRequestAsync(records));
        }

        /// <summary>
        /// Add a collection of records to the buffer before pushing.
        /// </summary>
        /// <param name="records">The records to add.</param>
        public void Push(ICollection<DataPoint> records)
        {
            Require.Instance.IsNotNull(records, nameof(records));

            log.Debug($"Adding [{records.Count}] records to the buffered sender.");
            this.statusTracker.Start();

            this.updateSender.AddRange(records.Where(r => r.OperationType == DataOperation.Update).Select(r => r.Record));
        }

        /// <summary>
        /// Dispose of the object's resources.
        /// </summary>
        public void Dispose()
        {
            this.client.Dispose();
            this.updateSender.Dispose();
            this.statusTracker.Dispose();
        }

        private async Task SendRequestAsync(IEnumerable<IDictionary<string, object>> records)
        {
            log.Info($"Sending Request to Bing Commerce Ingestion service.");
            try
            {
                var request = this.Serialize(records);
                var response = await this.client.PushDataUpdateAsync(request);

                if (response != null)
                {
                    this.statusTracker.Add(response.UpdateId, records);
                }
            }
            catch (Exception e)
            {
                log.Warn(e, $"Failed to push the data to bing commerce ingestion apis. Exception: {e.Message}");
            }
        }

        private string Serialize(IEnumerable<IDictionary<string, object>> records)
        {
            log.Debug($"Serializing records with the [{this.config.PushFormat.ToString()}] format.");
            return this.serializer.Serialize(records);
        }
    }
}
