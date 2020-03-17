// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bing.Commerce.Ingestion;
    using Microsoft.Bing.Commerce.Ingestion.Models;
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using NLog;

    /// <summary>
    /// A wrapper around the Bing for Commerce Ingestion client in with added retry option.
    /// </summary>
    public class BingCommerceIngestionWithRetry : IBingCommerceIngestion
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IBingCommerceIngestion internalClient;
        private readonly uint retryCount;
        private readonly uint initialBackoffMs;

        /// <summary>
        /// Initializes a new instance of the <see cref="BingCommerceIngestionWithRetry"/> class, which wraps the Bing for Commerce client with added retry logic.
        /// </summary>
        /// <param name="internalClient">The wrapped bing client for added retry option.</param>
        /// <param name="maxAttempts">The maximum number of attempts to call an API before failing the requests.</param>
        /// <param name="initialBackoffMs">The initial backoff wait time between attempts in milliseconds.</param>
        public BingCommerceIngestionWithRetry(IBingCommerceIngestion internalClient, uint maxAttempts, uint initialBackoffMs)
        {
            this.internalClient = internalClient;
            this.retryCount = maxAttempts;
            this.initialBackoffMs = initialBackoffMs;
        }

        /// <summary>
        /// Gets or sets property in inclosed object.
        /// </summary>
        Uri IBingCommerceIngestion.BaseUri { get => this.internalClient.BaseUri; set => this.internalClient.BaseUri = value; }

        /// <summary>
        /// Gets property in inclosed object.
        /// </summary>
        public JsonSerializerSettings SerializationSettings => this.internalClient.SerializationSettings;

        /// <summary>
        /// Gets property in inclosed object.
        /// </summary>
        public JsonSerializerSettings DeserializationSettings => this.internalClient.DeserializationSettings;

        /// <summary>
        /// Gets property in inclosed object.
        /// </summary>
        public ServiceClientCredentials Credentials => this.internalClient.Credentials;

        /// <summary>
        /// Create an index.
        /// </summary>
        /// <remarks>
        /// Creates a definition of the tenant's index.
        /// </remarks>
        /// <param name='tenantid'>The ID that uniquely identifies the tenant that the index belongs to.</param>
        /// <param name='subscriptionId'>Bing developer subcription id</param>
        /// <param name='body'>An Index object that describes the index definition to add.</param>
        /// <param name='customHeaders'>The headers that will be added to request.</param>
        /// <param name='cancellationToken'>The cancellation token.</param>
        /// <returns>The response from the inclosed object.</returns>
        public async Task<HttpOperationResponse<IndexResponse>> CreateIndexWithHttpMessagesAsync(string tenantid, string subscriptionId = null, Index body = null, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await RetryStrategy.RetryAsync(
                async () => await this.internalClient.CreateIndexWithHttpMessagesAsync(tenantid, subscriptionId, body, customHeaders, cancellationToken),
                this.retryCount,
                this.initialBackoffMs,
                (e) => this.ExceptionHandler(e, "CreateIndexWithHttpMessagesAsync", body.GetHashCode()));
        }

        /// <summary>
        /// Retrieve your current index's transformation config.
        /// </summary>
        /// <remarks>
        /// Retrieve the transformation config that currently applies to data you push to your index.
        /// </remarks>
        /// <param name='body'>The transformation config.</param>
        /// <param name='tenantid'>The ID that uniquely identifies the tenant that the index belongs to.</param>
        /// <param name='indexid'>The ID that uniquely identifies the index definition to manage.</param>
        /// <param name='subscriptionId'>Bing developer subcription id</param>
        /// <param name='customHeaders'>The headers that will be added to request.</param>
        /// <param name='cancellationToken'>The cancellation token.</param>
        /// <returns>The response from the inclosed object.</returns>
        public async Task<HttpOperationResponse<TransformationConfigResponse>> CreateOrUpdateTransformationConfigWithHttpMessagesAsync(string body, string tenantid, string indexid, string subscriptionId = null, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await RetryStrategy.RetryAsync(
                async () => await this.internalClient.CreateOrUpdateTransformationConfigWithHttpMessagesAsync(body, tenantid, indexid, subscriptionId, customHeaders, cancellationToken),
                this.retryCount,
                this.initialBackoffMs,
                (e) => this.ExceptionHandler(e, "CreateOrUpdateTransformationConfigWithHttpMessagesAsync", body.GetHashCode()));
        }

        /// <summary>
        /// Delete documents from your catalog.
        /// </summary>
        /// <remarks>
        /// Delete documents from your index's catalog.
        /// </remarks>
        /// <param name='tenantid'>The ID that uniquely identifies the tenant that the index belongs to.</param>
        /// <param name='indexid'>The ID that uniquely identifies the index definition to manage.</param>
        /// <param name='subscriptionId'>Bing developer subcription id.</param>
        /// <param name='body'>The set of document ids to delete.</param>
        /// <param name='customHeaders'>The headers that will be added to request.</param>
        /// <param name='cancellationToken'>The cancellation token.</param>
        /// <returns>The response from the inclosed object.</returns>
        public async Task<HttpOperationResponse<DeleteDocumentsResponse>> DeleteDocumentsWithHttpMessagesAsync(string tenantid, string indexid, string subscriptionId = null, RequestsStringSet body = null, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await RetryStrategy.RetryAsync(
                async () => await this.internalClient.DeleteDocumentsWithHttpMessagesAsync(tenantid, indexid, subscriptionId, body, customHeaders, cancellationToken),
                this.retryCount,
                this.initialBackoffMs,
                (e) => this.ExceptionHandler(e, "DeleteDocumentsWithHttpMessagesAsync", body.GetHashCode()));
        }

        /// <summary>
        /// Delete an index.
        /// </summary>
        /// <remarks>
        /// Delete an index definition along with all the catalog.
        /// </remarks>
        /// <param name='tenantid'>The ID that uniquely identifies the tenant that the index belongs to.</param>
        /// <param name='indexid'>The ID that uniquely identifies the index definition to manage.</param>
        /// <param name='subscriptionId'>Bing developer subcription id.</param>
        /// <param name='customHeaders'>The headers that will be added to request.</param>
        /// <param name='cancellationToken'>The cancellation token.</param>
        /// <returns>The response from the inclosed object.</returns>
        public async Task<HttpOperationResponse<IndexResponse>> DeleteIndexWithHttpMessagesAsync(string tenantid, string indexid, string subscriptionId = null, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await RetryStrategy.RetryAsync(
                async () => await this.internalClient.DeleteIndexWithHttpMessagesAsync(tenantid, indexid, subscriptionId, customHeaders, cancellationToken),
                this.retryCount,
                this.initialBackoffMs,
                (e) => this.ExceptionHandler(e, "DeleteIndexWithHttpMessagesAsync", indexid.GetHashCode()));
        }

        /// <summary>
        /// Delete your index's transformation config.
        /// </summary>
        /// <remarks>
        /// Delete the transformation config that currently aplies to data you push to your index.
        /// </remarks>
        /// <param name='tenantid'>The ID that uniquely identifies the tenant that the index belongs to.</param>
        /// <param name='indexid'>The ID that uniquely identifies the index definition to manage.</param>
        /// <param name='subscriptionId'>Bing developer subcription id.</param>
        /// <param name='customHeaders'>The headers that will be added to request.</param>
        /// <param name='cancellationToken'>The cancellation token.</param>
        /// <returns>The response from the inclosed object.</returns>
        public async Task<HttpOperationResponse<TransformationConfigResponse>> DeleteTransformationConfigWithHttpMessagesAsync(string tenantid, string indexid, string subscriptionId = null, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await RetryStrategy.RetryAsync(
                async () => await this.internalClient.DeleteTransformationConfigWithHttpMessagesAsync(tenantid, indexid, subscriptionId, customHeaders, cancellationToken),
                this.retryCount,
                this.initialBackoffMs,
                (e) => this.ExceptionHandler(e, "DeleteTransformationConfigWithHttpMessagesAsync", indexid.GetHashCode()));
        }

        /// <summary>
        /// Detect the data schema.
        /// </summary>
        /// <param name='subscriptionId'>Bing developer subcription id.</param>
        /// <param name='format'>
        /// The push data update document format. Possible values include:
        /// 'Unknown', 'LDJson', 'CSV', 'TSV', 'JsonArray'
        /// </param>
        /// <param name='customHeaders'>The headers that will be added to request.</param>
        /// <param name='cancellationToken'>The cancellation token.</param>
        /// <returns>The response from the inclosed object.</returns>
        public async Task<HttpOperationResponse<SchemaDetectionResponse>> DetectSchemaWithHttpMessagesAsync(string subscriptionId = null, string format = null, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await RetryStrategy.RetryAsync(
                async () => await this.internalClient.DetectSchemaWithHttpMessagesAsync(subscriptionId, format, customHeaders, cancellationToken),
                this.retryCount,
                this.initialBackoffMs,
                (e) => this.ExceptionHandler(e, "DetectSchemaWithHttpMessagesAsync", 0));
        }

        /// <summary>
        /// Disposes nothing as the internal client is not owned by this.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Upload a trytout config.
        /// </summary>
        /// <remarks>
        /// Upload a transformation config that you can use to test data
        /// transformation on the cloud.
        /// </remarks>
        /// <param name='body'>The transformation config.</param>
        /// <param name='tryoutid'>The transformation tryout config id.</param>
        /// <param name='subscriptionId'>Bing developer subcription id.</param>
        /// <param name='customHeaders'>The headers that will be added to request.</param>
        /// <param name='cancellationToken'>The cancellation token.</param>
        /// <returns>The response from the inclosed object.</returns>
        public async Task<HttpOperationResponse<TransformationTryoutResponse>> ExecuteTryOutConfigWithHttpMessagesAsync(string body, string tryoutid, string subscriptionId = null, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await RetryStrategy.RetryAsync(
                async () => await this.internalClient.ExecuteTryOutConfigWithHttpMessagesAsync(body, tryoutid, subscriptionId, customHeaders, cancellationToken),
                this.retryCount,
                this.initialBackoffMs,
                (e) => this.ExceptionHandler(e, "ExecuteTryOutConfigWithHttpMessagesAsync", body.GetHashCode()));
        }

        /// <summary>
        /// Get list of index definitions.
        /// </summary>
        /// <remarks>
        /// Get list of index definitions that you defined for a tenant.
        /// </remarks>
        /// <param name='tenantid'>The ID that uniquely identifies the tenant that the index belongs to.</param>
        /// <param name='subscriptionId'>Bing developer subcription id.</param>
        /// <param name='customHeaders'>The headers that will be added to request.</param>
        /// <param name='cancellationToken'>The cancellation token.</param>
        /// <returns>The response from the inclosed object.</returns>
        public async Task<HttpOperationResponse<IndexResponse>> GetAllIndexesWithHttpMessagesAsync(string tenantid, string subscriptionId = null, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await RetryStrategy.RetryAsync(
                async () => await this.internalClient.GetAllIndexesWithHttpMessagesAsync(tenantid, subscriptionId, customHeaders, cancellationToken),
                this.retryCount,
                this.initialBackoffMs,
                (e) => this.ExceptionHandler(e, "GetAllIndexesWithHttpMessagesAsync", tenantid.GetHashCode()));
        }

        /// <summary>
        /// Get index status by id.
        /// </summary>
        /// <remarks>
        /// Get the detailed status of your index in each supported region.
        /// </remarks>
        /// <param name='tenantid'>The ID that uniquely identifies the tenant that the index belongs to.</param>
        /// <param name='indexid'>The ID that uniquely identifies the index definition to manage.</param>
        /// <param name='subscriptionId'>Bing developer subcription id.</param>
        /// <param name='customHeaders'>The headers that will be added to request.</param>
        /// <param name='cancellationToken'>The cancellation token.</param>
        /// <returns>The response from the inclosed object.</returns>
        public async Task<HttpOperationResponse<IndexStatusResponse>> GetIndexStatusWithHttpMessagesAsync(string tenantid, string indexid, string subscriptionId = null, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await RetryStrategy.RetryAsync(
                async () => await this.internalClient.GetIndexStatusWithHttpMessagesAsync(tenantid, indexid, subscriptionId, customHeaders, cancellationToken),
                this.retryCount,
                this.initialBackoffMs,
                (e) => this.ExceptionHandler(e, "GetIndexStatusWithHttpMessagesAsync", indexid.GetHashCode()));
        }

        /// <summary>
        /// Get index definition by id.
        /// </summary>
        /// <remarks>
        /// Get a specific index definition for a tenant.
        /// </remarks>
        /// <param name='tenantid'>The ID that uniquely identifies the tenant that the index belongs to.</param>
        /// <param name='indexid'>The ID that uniquely identifies the index definition to manage.</param>
        /// <param name='subscriptionId'>Bing developer subcription id.</param>
        /// <param name='customHeaders'>The headers that will be added to request.</param>
        /// <param name='cancellationToken'>The cancellation token.</param>
        /// <returns>The response from the inclosed object.</returns>
        public async Task<HttpOperationResponse<IndexResponse>> GetIndexWithHttpMessagesAsync(string tenantid, string indexid, string subscriptionId = null, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await RetryStrategy.RetryAsync(
                async () => await this.internalClient.GetIndexWithHttpMessagesAsync(tenantid, indexid, subscriptionId, customHeaders, cancellationToken),
                this.retryCount,
                this.initialBackoffMs,
                (e) => this.ExceptionHandler(e, "GetIndexWithHttpMessagesAsync", indexid.GetHashCode()));
        }

        /// <summary>
        /// Get ingestion status.
        /// </summary>
        /// <remarks>
        /// Track your ingestion by querying the status.
        /// </remarks>
        /// <param name='tenantid'>The ID that uniquely identifies the tenant that the index belongs to.</param>
        /// <param name='indexid'>The ID that uniquely identifies the index definition to manage.</param>
        /// <param name='subscriptionId'>Bing developer subcription id.</param>
        /// <param name='customHeaders'>The headers that will be added to request.</param>
        /// <param name='cancellationToken'>The cancellation token.</param>
        /// <returns>The response from the inclosed object.</returns>
        public async Task<HttpOperationResponse<TransformationConfigResponse>> GetTransformationConfigWithHttpMessagesAsync(string tenantid, string indexid, string subscriptionId = null, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await RetryStrategy.RetryAsync(
                async () => await this.internalClient.GetTransformationConfigWithHttpMessagesAsync(tenantid, indexid, subscriptionId, customHeaders, cancellationToken),
                this.retryCount,
                this.initialBackoffMs,
                (e) => this.ExceptionHandler(e, "GetTransformationConfigWithHttpMessagesAsync", indexid.GetHashCode()));
        }

        /// <summary>
        /// Query the status for a push update using the update id.
        /// </summary>
        /// <param name='tenantid'>The ID that uniquely identifies the tenant that the index belongs to.</param>
        /// <param name='indexid'>The ID that uniquely identifies the index definition to manage.</param>
        /// <param name='updateid'>An id to uniquely identify the push update request in order to be able to track it down later.</param>
        /// <param name='subscriptionId'>Bing developer subcription id.</param>
        /// <param name='customHeaders'>The headers that will be added to request.</param>
        /// <param name='cancellationToken'>The cancellation token.</param>
        /// <returns>The response from the inclosed object.</returns>
        public async Task<HttpOperationResponse<PushUpdateStatusResponse>> PushDataStatusWithHttpMessagesAsync(string tenantid, string indexid, string updateid, string subscriptionId = null, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await RetryStrategy.RetryAsync(
                async () => await this.internalClient.PushDataStatusWithHttpMessagesAsync(tenantid, indexid, updateid, subscriptionId, customHeaders, cancellationToken),
                this.retryCount,
                this.initialBackoffMs,
                (e) => this.ExceptionHandler(e, "PushDataStatusWithHttpMessagesAsync", updateid.GetHashCode()));
        }

        /// <summary>
        /// Push catalog data.
        /// </summary>
        /// <remarks>
        /// This method pushes updates to your your index data to Bing. This is
        /// an asynchronous process. To upload your index data to Bing, you'll
        /// send a push request that contains your index data.
        /// </remarks>
        /// <param name='body'>The data to be ingested.</param>
        /// <param name='tenantid'>The ID that uniquely identifies the tenant that the index belongs to.</param>
        /// <param name='indexid'>The ID that uniquely identifies the index definition to manage.</param>
        /// <param name='subscriptionId'>Bing developer subcription id.</param>
        /// <param name='notransform'>
        /// This disables transformation config processing if the config was
        /// uploaded for some index. It's useful when the data is already
        /// transformed and is matching the index definition. Default is `false`
        /// </param>
        /// <param name='updateid'>An id to uniquely identify the push update request in order to beable to track it down later.</param>
        /// <param name='customHeaders'>The headers that will be added to request.</param>
        /// <param name='cancellationToken'>The cancellation token.</param>
        /// <returns>The response from the inclosed object.</returns>
        public async Task<HttpOperationResponse<PushDataUpdateResponse>> PushDataUpdateWithHttpMessagesAsync(string body, string tenantid, string indexid, string subscriptionId = null, bool? notransform = null, string updateid = null, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await RetryStrategy.RetryAsync(
                async () => await this.internalClient.PushDataUpdateWithHttpMessagesAsync(body, tenantid, indexid, subscriptionId, notransform, updateid, customHeaders, cancellationToken),
                this.retryCount,
                this.initialBackoffMs,
                (e) => this.ExceptionHandler(e, "PushDataUpdateWithHttpMessagesAsync", body.GetHashCode()));
        }

        /// <summary>
        /// Updates an index with id.
        /// </summary>
        /// <remarks>
        /// Update the definition for your index.
        /// </remarks>
        /// <param name='tenantid'>The ID that uniquely identifies the tenant that the index belongs to.</param>
        /// <param name='indexid'>The ID that uniquely identifies the index definition to manage.</param>
        /// <param name='subscriptionId'>Bing developer subcription id.</param>
        /// <param name='body'>The upated index information.</param>
        /// <param name='customHeaders'>The headers that will be added to request.</param>
        /// <param name='cancellationToken'>The cancellation token.</param>
        /// <returns>The response from the inclosed object.</returns>
        public async Task<HttpOperationResponse<IndexResponse>> UpdateIndexWithHttpMessagesAsync(string tenantid, string indexid, string subscriptionId = null, Index body = null, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await RetryStrategy.RetryAsync(
                async () => await this.internalClient.UpdateIndexWithHttpMessagesAsync(tenantid, indexid, subscriptionId, body, customHeaders, cancellationToken),
                this.retryCount,
                this.initialBackoffMs,
                (e) => this.ExceptionHandler(e, "UpdateIndexWithHttpMessagesAsync", body.GetHashCode()));
        }

        /// <summary>
        /// Create a transformation config.
        /// </summary>
        /// <remarks>
        /// Upload a new transformation config and set it as the transformation
        /// config that applies to data you push to your index.
        /// </remarks>
        /// <param name='body'>The transformation config.</param>
        /// <param name='subscriptionId'>Bing developer subcription id</param>
        /// <param name='customHeaders'>The headers that will be added to request.</param>
        /// <param name='cancellationToken'>The cancellation token.</param>
        /// <returns>The response from the inclosed object.</returns>
        public async Task<HttpOperationResponse<TransformationConfigResponse>> UploadTryOutConfigWithHttpMessagesAsync(string body, string subscriptionId = null, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await RetryStrategy.RetryAsync(
                async () => await this.internalClient.UploadTryOutConfigWithHttpMessagesAsync(body, subscriptionId, customHeaders, cancellationToken),
                this.retryCount,
                this.initialBackoffMs,
                (e) => this.ExceptionHandler(e, "UploadTryOutConfigWithHttpMessagesAsync", body.GetHashCode()));
        }

        private void ExceptionHandler(Exception e, string apiName, int apiInputRef)
        {
            if (e is HttpOperationException)
            {
                var httpException = (HttpOperationException)e;
                var x_msedge_ref = string.Empty;
                if (httpException.Response.Headers.TryGetValue("X-MSEdge-Ref", out var headers))
                {
                    x_msedge_ref = headers.First();
                }

                log.Warn(httpException, $"Received status code [{httpException.Response.StatusCode}] when sending the request to [{apiName}] with input hash [{apiInputRef}]. X-MSEdge-Ref from response: [{x_msedge_ref}]");
            }
        }
    }
}