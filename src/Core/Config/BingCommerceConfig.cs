// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.Core.Config
{
    using System;
    using Microsoft.Bing.Commerce.Connectors.Core.Utilities;

    /// <summary>
    /// The configuration needed for the Bing for Commerce pusher.
    /// </summary>
    public class BingCommerceConfig
    {
        private static readonly string ACCESS_TOKEN_VAR_NAME = "ACCESS_TOKEN";

        /// <summary>
        /// Gets or sets the tenant id to push the data to.
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets the index to push the data to.
        /// </summary>
        public string IndexId { get; set; }

        /// <summary>
        /// Gets or sets the Bearer access token to use when pushing the data. Will get from the environment variable `ACCESS_TOKEN` if missing.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the format to use when pushing the data to the Bing for Commerce endpoint.
        /// </summary>
        public Format PushFormat { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of records per push request.
        /// </summary>
        public uint MaxBatchCount { get; set; }

        /// <summary>
        /// Gets or sets the maximum size of the request sent to the Bing API in bytes.
        /// </summary>
        public uint MaxRequestSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of concurrent requests to use when pushing.
        /// </summary>
        public uint MaxConcurrentRequests { get; set; }

        /// <summary>
        /// Gets or sets the desired level for logging the full requests.
        /// </summary>
        public RequestLogLevel RequestLog { get; set; }

        /// <summary>
        /// Gets or sets the directory you wish to store the full requests.
        /// </summary>
        public string RequestLogLocation { get; set; }

        /// <summary>
        /// Gets or sets the interval on which to track the push update requests
        /// </summary>
        public TimeSpan? TrackingInterval { get; set; }

        /// <summary>
        /// Gets or sets number of retries before deadlettering the request.
        /// </summary>
        public uint RetryCount { get; set; }

        /// <summary>
        /// Gets or sets the maximum wait in-between events to wait for before pushing (For the buffered pusher).
        /// </summary>
        public uint MaxBufferWaitMs { get; set; }

        /// <summary>
        /// Validates the configurations are within proper boundaries.
        /// </summary>
        /// <returns>this</returns>
        public BingCommerceConfig Check()
        {
            var maxBatchCount = this.MaxBatchCount;
            var accessToken = this.AccessToken;
            Require.Instance.IsNotNull(this.TenantId, nameof(this.TenantId))
                .IsNotNull(this.IndexId, nameof(this.IndexId))
                .IsTrue(this.MaxBatchCount < 10000, "Max Batch size can't be more than 1000")
                .IsTrue(this.MaxRequestSize < 250 * 1024 * 1024, "Max Request Size in bytes can't exceed 250 MB")
                .IsTrue(this.MaxConcurrentRequests < 1000, "Max concurrent requests can't exceed 1000")
                .IsTrue(this.RequestLog == RequestLogLevel.None || this.RequestLogLocation != null, "Request log location can't be null when request logs are not set to None.")
                .IsTrue(this.RetryCount < 10, "Retry count can't exceed 10")
                .IsTrue(this.MaxBufferWaitMs < 3600000, "Max buffer wait can't exceed one hour")
                .OrDefault(ref maxBatchCount, 0u, 1000u)
                .OrDefault(ref accessToken, string.IsNullOrEmpty(accessToken), Environment.GetEnvironmentVariable(ACCESS_TOKEN_VAR_NAME))
                .IsTrue(!string.IsNullOrEmpty(accessToken), "access token can't be null or empty");

            this.MaxBatchCount = maxBatchCount;
            this.AccessToken = accessToken;

            return this;
        }
    }
}
