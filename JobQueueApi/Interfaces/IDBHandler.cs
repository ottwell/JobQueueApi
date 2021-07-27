using JobQueueApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobQueueApi.Interfaces
{
    public interface IDBHandler
    {
        public Task<List<Job>> GetAll();

        public Task<Job> GetById(Guid jobId);

        public Task<Job> Insert(int[] input);

        public Task Update(Job job);
    }
}
