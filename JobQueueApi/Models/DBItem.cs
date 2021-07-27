using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobQueueApi.Models
{
    public class DBItem
    {
        public Guid Id { get; set; }
        public DateTime Created { get; set; }
    }
}
