using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobQueueApi.Enums
{
    public enum JobStatus
    {
        Queued, InProgress, Completed, Aborted
    }
}
