// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.Core.Utilities
{
    using System;
    using System.Threading.Tasks;
    using NLog;

    /// <summary>
    /// A static utility class to help us retry operations in case of failure.
    /// </summary>
    public static class RetryStrategy
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Attempt to invoke the given function until it succeeds, or the maximum number of attempts have been reached.
        /// </summary>
        /// <typeparam name="T">The return type of the given function</typeparam>
        /// <param name="func">The function to attempt to invoke.</param>
        /// <param name="maxAttempts">The maximum number of attempts to invoke the given function. Has to be greater than zero.</param>
        /// <param name="initialBackoffMs">The initial backoff time wait between attempts in milliseconds. Has to be greater than zero.</param>
        /// <param name="exceptionCallback">(Optional): A callback to be invoked when the operation fails at any attempt.</param>
        /// <returns>Forwards the return value from the given function.</returns>
        public static async Task<T> RetryAsync<T>(Func<Task<T>> func, uint maxAttempts, uint initialBackoffMs, Action<Exception> exceptionCallback = null)
        {
            Require.Instance.IsNotNull(func, nameof(func))
                .IsTrue(maxAttempts > 0, "the maximum number of attempt has to be greater than zero")
                .IsTrue(initialBackoffMs > 0, "the initial backoff time in milliseconds has to be greater than zero");

            for (uint attempt = 0, backoff = initialBackoffMs; attempt < maxAttempts; attempt++, backoff += backoff)
            {
                try
                {
                    var response = await func();
                    return response;
                }
                catch (Exception e)
                {
                    exceptionCallback?.Invoke(e);

                    if (attempt != maxAttempts - 1)
                    {
                        log.Warn(e, $"Failed attempt number [{attempt}] to perform the action. Retrying after a backoff of [{backoff}] ms.");
                        await Task.Delay((int)backoff);
                    }
                    else
                    {
                        log.Error(e, $"Failed all attempts to perform the action.");
                        throw e;
                    }
                }
            }

            return default(T);
        }
    }
}
