// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("BingCommerceConnectorCore.Tests")]
namespace Microsoft.Bing.Commerce.Connectors.Core.Utilities
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using NLog;

    /// <summary>
    /// A utility class that provides a way to schedule a recurring task with a specific interval.
    /// </summary>
    public class BackgroundTaskScheduler
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly TimeSpan interval;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundTaskScheduler"/> class.
        /// </summary>
        /// <param name="interval">The interval to run the task at.</param>
        public BackgroundTaskScheduler(TimeSpan interval)
        {
            Require.Instance.IsTrue(interval.TotalMilliseconds > 10, "The cadence cannot be less than 10ms");

            this.interval = interval;

            log.Debug($"Successfully created the Background Task Scheduler with interval: {interval.ToString()}.");
        }

        /// <summary>
        /// Start the scheduler run.
        /// </summary>
        /// <param name="action">The action to perform on the scheduler's stops.</param>
        /// <param name="cancellation">(optional): the cancellation token that you can use to stop the run.</param>
        /// <returns>The running task for the scheduler.</returns>
        public async Task StartAsync(Action action, CancellationToken cancellation = default(CancellationToken))
        {
            Require.Instance.IsNotNull(action, nameof(action));
            log.Info($"Starting the background task scheduler with cadence: {this.interval.ToString()}.");

            await Task.Run(async () =>
            {
                Task currentTask = null;
                while (!cancellation.IsCancellationRequested)
                {
                    await Task.Delay(this.interval);

                    if (!cancellation.IsCancellationRequested && (currentTask == null || currentTask.IsCompleted))
                    {
                        currentTask = Task.Run(action);
                    }
                }
                await currentTask;
            });
        }
    }
}
