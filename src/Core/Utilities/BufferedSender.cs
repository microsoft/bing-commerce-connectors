// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Timers;

    internal class BufferedSender<TRecord> : IDisposable
    {
        private readonly RequestList<TRecord> list;
        private readonly Func<IEnumerable<TRecord>, Task> processor;
        private readonly TaskManager taskManager;

        private readonly Timer timer;

        private readonly object listLock = new object();

        public BufferedSender(uint maxBufferWaitMs, TaskManager taskManager, RequestList<TRecord> list, Func<IEnumerable<TRecord>, Task> processor)
        {
            this.list = list;
            this.processor = processor;
            this.taskManager = taskManager;

            this.timer = new Timer(maxBufferWaitMs);
            this.timer.AutoReset = false;
            this.timer.Elapsed += this.TimerCallback;
        }

        public void Add(TRecord record)
        {
            if (!this.AddInternal(record))
            {
                this.timer.Start();
            }
        }

        public void AddRange(IEnumerable<TRecord> input)
        {
            bool shouldSchedule = false;
            foreach (var record in input)
            {
                shouldSchedule = !this.AddInternal(record);
            }

            if (shouldSchedule)
            {
                this.timer.Start();
            }
        }

        public void Dispose()
        {
            this.timer.Dispose();
        }

        private bool AddInternal(TRecord entry)
        {
            this.timer.Stop();

            List<TRecord> localRecords = null;
            lock (this.listLock)
            {
                localRecords = this.list.Add(entry);
            }

            return this.InvokeProcessor(localRecords);
        }

        private void TimerCallback(object sender, ElapsedEventArgs e)
        {
            List<TRecord> localRecords = null;
            lock (this.listLock)
            {
                localRecords = this.list.ResetList();
            }

            this.InvokeProcessor(localRecords);
        }

        private bool InvokeProcessor(List<TRecord> localRecords)
        {
            if (localRecords != null && localRecords.Count > 0)
            {
                this.taskManager.Add(() => this.processor(localRecords));
                return true;
            }

            return false;
        }
    }
}
