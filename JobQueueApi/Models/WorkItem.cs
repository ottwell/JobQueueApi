using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobQueueApi.Models
{
    public class WorkItem
    {
        public Guid Id { get; set; }

        public Func<Task> Action { get; set; }

    }
}
