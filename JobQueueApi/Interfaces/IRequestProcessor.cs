using JobQueueApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobQueueApi.Interfaces
{
    public interface IRequestProcessor
    {
        public Task<List<Job>> GetAll();

        public Task<Job> GetById(Guid jobId);

        public Task<Job> CreateJob(int[] input);

        public Task ProcessJob(Job job);
    }
}
