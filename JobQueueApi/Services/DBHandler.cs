using JobQueueApi.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JobQueueApi.Interfaces;

namespace JobQueueApi.Services
{
    public class DBHandler : IDBHandler, IDisposable
    {
        private readonly JobContext _context;

        private bool _disposed;

        private ILogger _logger;

        public DBHandler(JobContext context, ILoggerFactory logFactory)
        {
            _context = context;
            _logger = logFactory.CreateLogger<DBHandler>();
        }


        public async Task<List<Job>> GetAll()
        {
            return await _context.Jobs.ToListAsync();
        }

        public async Task<Job> GetById(Guid jobId)
        {
            return await _context.Jobs.FindAsync(jobId);
        }

        public async Task<Job> Insert(int[] input)
        {
            var job = new Job(input);
            _context.Jobs.Add(job);
            _logger.LogInformation($"created new job with id {job.Id} and input array {string.Join(',', input)}. will save to DB");
            await _context.SaveChangesAsync();
            return job;
        }

        public async Task Update(Job job)
        {
            _context.Entry(job).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
        }
    }
}
