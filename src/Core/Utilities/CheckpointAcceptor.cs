// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.Core.Utilities
{
    using System.Collections.Generic;

    internal class CheckpointAcceptor
    {
        private readonly Queue<CheckpointEntry> checkpointsInOrder = new Queue<CheckpointEntry>();
        private readonly HashSet<CheckpointEntry> checkpointsSet = new HashSet<CheckpointEntry>();

        private readonly object stateLock = new object();

        public CheckpointAcceptor(IDataCheckpoint checkpoint)
        {
            Require.Instance.IsNotNull(checkpoint, nameof(checkpoint));

            this.Checkpoint = checkpoint;
        }

        public IDataCheckpoint Checkpoint { get; }

        public void Pending(string pending)
        {
            Require.Instance.IsTrue(!string.IsNullOrEmpty(pending), "pending checkpoint cannot be null or empty");

            var current = new CheckpointEntry(pending);

            // No need to lock, given that this is expected to be called from a single thread,
            // and that the state is inserted at the other end of the queue.
            this.checkpointsInOrder.Enqueue(current);
            this.checkpointsSet.Add(current);
        }

        public void Accept(string accepted)
        {
            Require.Instance.IsTrue(!string.IsNullOrEmpty(accepted), "accepted checkpoint cannot be null or empty");

            this.checkpointsSet.TryGetValue(accepted, out CheckpointEntry checkpoint);
            Require.Instance.State(checkpoint != null, "accepted checkpoint is not found in pending ones.");
            checkpoint.Accepted = true;

            var latestAccepted = EjectAcceptedCheckpoints();

            if (latestAccepted != null)
            {
                this.Checkpoint.Accept(latestAccepted);
            }
        }

        private string EjectAcceptedCheckpoints()
        {
            string latestAccepted = null;

            lock (this.stateLock)
            {
                // Locking is needed to make sure the checkpoint peeked is the checkpoint being dequeued.
                while (this.checkpointsInOrder.Count > 0 && this.checkpointsInOrder.Peek().Accepted)
                {
                    var current = this.checkpointsInOrder.Dequeue();
                    this.checkpointsSet.Remove(current);
                    latestAccepted = current.Checkpoint;
                }
            }

            return latestAccepted;
        }

        internal class CheckpointEntry
        {
            public CheckpointEntry(string checkpoint)
            {
                this.Checkpoint = checkpoint;
            }

            public string Checkpoint { get; set; }

            public bool Accepted { get; set; }

            public static implicit operator CheckpointEntry(string checkpoint)
            {
                return new CheckpointEntry(checkpoint);
            }

            public override int GetHashCode()
            {
                return this.Checkpoint.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return (obj as CheckpointEntry)?.Checkpoint == this.Checkpoint;
            }
        }
    }
}
