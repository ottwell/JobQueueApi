using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using JobQueueApi.Interfaces;
using JobQueueApi.Models;

namespace JobQueueApi.Services
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private ConcurrentQueue<WorkItem> _workItems =
        new ConcurrentQueue<WorkItem>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void QueueBackgroundWorkItem(WorkItem workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }
            _workItems.Enqueue(workItem);
            _signal.Release();
        }

        public async Task<Func<Task>> DequeueAsync()
        {
            await _signal.WaitAsync();
            _workItems.TryDequeue(out var workItem);

            return workItem.Action;
        }

        public int GetCurrentQueueLength()
        {
            return _workItems.Count;
        }

        public int GetCurrentItemQueuePosition(Guid itemId)
        {
            return _workItems.Select(item => item.Id).ToList().FindIndex(id => id == itemId);
        }




    }
}
