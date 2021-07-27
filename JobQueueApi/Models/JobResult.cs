using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobQueueApi.Models
{
    public class JobResult
    {
        public Job Job { get; set; }
        public int CurrentPlaceInQueue { get; set; }
    }
}
