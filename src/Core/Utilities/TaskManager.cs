// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    internal class TaskManager
    {
        private readonly uint maximumTasks;
        private List<Task> tasks = new List<Task>();

        public TaskManager(uint maximumTasks)
        {
            this.maximumTasks = maximumTasks;
        }

        public void Add(Func<Task> task)
        {
            this.FilterTasks();
            this.tasks.Add(task());
        }

        public void WaitAll()
        {
            Task.WaitAll(this.tasks.ToArray());
        }

        private void FilterTasks()
        {
            while (this.tasks.Count >= this.maximumTasks)
            {
                Task.WaitAny(this.tasks.ToArray());
                this.tasks = this.tasks.Where(t => !t.IsCompleted).ToList();
            }
        }
    }
}
