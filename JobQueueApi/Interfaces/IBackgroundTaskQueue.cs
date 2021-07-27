using JobQueueApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobQueueApi.Interfaces
{
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundWorkItem(WorkItem workItem);

        Task<Func<Task>> DequeueAsync();

        int GetCurrentQueueLength();

        int GetCurrentItemQueuePosition(Guid jobId);
    }
}
