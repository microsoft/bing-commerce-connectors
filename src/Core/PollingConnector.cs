// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.Core
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bing.Commerce.Connectors.Core.Config;
    using Microsoft.Bing.Commerce.Connectors.Core.Utilities;
    using NLog;

    /// <summary>
    /// A Bing for Commerce Connector that polls data from the given data source in a configurable interval.
    /// </summary>
    public class PollingConnector
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly PollingConnectorConfig config;
        private readonly IDataPusher pusher;
        private readonly IDataReader db;
        private readonly IDataCheckpoint checkpoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="PollingConnector"/> class.
        /// </summary>
        /// <param name="config">The polling connector configuration.</param>
        /// <param name="dataAccess">The data source to poll data from.</param>
        /// <param name="checkpoint">The checkpoint object to start polling data since.</param>
        /// <param name="pusher">The Bing for Commerce data pusher.</param>
        public PollingConnector(PollingConnectorConfig config, IDataReader dataAccess, IDataCheckpoint checkpoint, IDataPusher pusher)
        {
            this.config = config;
            this.pusher = pusher;
            this.db = dataAccess;
            this.checkpoint = checkpoint;

            log.Debug($"Successfully created the polling connector.");
        }

        /// <summary>
        /// Start running the connector. It first does an initial push for data since the provided checkpoint, and then starts the background job cadence.
        /// </summary>
        /// <param name="cancellationToken">The task cancelation token.</param>
        /// <returns>The running task</returns>
        public async Task RunAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            log.Info("Put on your seat-belt, starting the polling connector run.");

            log.Info($"Doing an initial poll with the checkpoint: {this.checkpoint.GetValue()}.");
            this.pusher.Push(this.db);
            if (this.config?.ScanInterval.TotalMilliseconds > 0)
            {
                var scheduler = new BackgroundTaskScheduler(this.config.ScanInterval);
                await scheduler.StartAsync(() => this.pusher.Push(this.db), cancellationToken);
            }
        }
    }
}
