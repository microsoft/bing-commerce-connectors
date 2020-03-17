// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.Core
{
    /// <summary>
    /// An interface that describes the a marker to start polling the data after (e.g. a timestamp, change id, ..etc).
    /// </summary>
    public interface IDataCheckpoint
    {
        /// <summary>
        /// This would be false for the first connector run, which means that the connector should poll all data.
        /// </summary>
        /// <returns>true if the current checkpoint hold a valid checkpoint value, false if it doesn't (first poll).</returns>
        bool IsValid();

        /// <summary>
        /// Gets the value of the current checkpoint as a string.
        /// </summary>
        /// <returns>The value of the checkpoint</returns>
        string GetValue();

        /// <summary>
        /// Accept the changes, and move the checkpoint to the new given checkpoint value.
        /// </summary>
        /// <param name="newCheckpoint">The new checkpoint value to move the current checkpoint to.</param>
        void Accept(string newCheckpoint);
    }
}
