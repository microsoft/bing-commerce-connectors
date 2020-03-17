// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.Core.Utilities
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bing.Commerce.Ingestion;
    using Microsoft.Bing.Commerce.Ingestion.Models;
    using NLog;

    internal class IngestionClient : IDisposable
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly BingCommerceIngestion client;
        private readonly BingCommerceIngestionWithRetry clientWithRetry;
        private readonly uint retryCount;
        private readonly RequestLogger logger;

        public IngestionClient(BingCommerceIngestion client, string tenantid, string indexid, uint maxAttempts, RequestLogger logger)
        {
            Require.Instance.IsNotNull(client, nameof(client))
                .IsNotNull(logger, nameof(logger))
                .IsNotNull(tenantid, nameof(tenantid))
                .IsNotNull(indexid, nameof(indexid))
                .IsTrue(maxAttempts > 0, "max attempts cannot be zero or less.");

            this.clientWithRetry = new BingCommerceIngestionWithRetry(client, maxAttempts, 500);

            this.client = client;
            this.Tenantid = tenantid;
            this.Indexid = indexid;
            this.retryCount = maxAttempts;
            this.logger = logger;
        }

        public string Tenantid { get; }

        public string Indexid { get; }

        public async Task<PushDataUpdateResponse> PushDataUpdateAsync(string body, CancellationToken cancellation = default(CancellationToken))
        {
            Require.Instance.IsNotNull(body, nameof(body));

            try
            {
                var response = await this.clientWithRetry.PushDataUpdateAsync(body, this.Tenantid, this.Indexid, cancellationToken: cancellation);
                this.logger.LogSuccess(body);
                return response;
            }
            catch
            {
                this.logger.LogFailure(body);
                return null;
            }
        }

        public async Task<PushUpdateStatusResponse> PushDataStatusAsync(string updateid, CancellationToken cancellation = default(CancellationToken))
        {
            Require.Instance.IsNotNull(updateid, nameof(updateid));

            try
            {
                return await this.client.PushDataStatusAsync(this.Tenantid, this.Indexid, updateid, cancellationToken: cancellation);
            }
            catch
            {
                log.Warn($"Failed at a best effort to query push data status. Will retry at next schedule.");
                return null;
            }
        }

        public async Task<IndexResponse> GetIndexAsync(CancellationToken cancellation = default(CancellationToken))
        {
            return await RetryStrategy.RetryAsync(async () => await this.client.GetIndexAsync(this.Tenantid, this.Indexid, cancellationToken: cancellation), this.retryCount, 500);
        }

        public void Dispose()
        {
            this.clientWithRetry.Dispose();
        }
    }
}
