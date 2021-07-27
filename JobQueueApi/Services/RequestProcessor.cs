using JobQueueApi.Enums;
using JobQueueApi.Helpers;
using JobQueueApi.Interfaces;
using JobQueueApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobQueueApi.Services
{
    public class RequestProcessor : IRequestProcessor, IDisposable
    {
        private IDBHandler _dbHandler;
        private bool _disposed;
        private ILogger _logger;
        private IConfiguration _config;

        public RequestProcessor(IDBHandler handler, ILoggerFactory logFactory, IConfiguration config)
        {
            _dbHandler = handler;
            _logger = logFactory.CreateLogger<RequestProcessor>();
            _config = config;
        }

        public async Task<List<Job>> GetAll()
        {
            var jobs = await _dbHandler.GetAll();
            _logger.LogInformation($"retrieved {jobs.Count} jobs from DB. Will return");
            return jobs;
        }

        public async Task<Job> GetById(Guid jobId)
        {
            var job = await _dbHandler.GetById(jobId);
            if (job == null)
            {
                _logger.LogInformation($"found no jobs in DB that match the given id: {jobId}.");
                return null;
            }
            _logger.LogInformation($"found 1 jobs in DB that matches the given id: {jobId}. Will return");
            return job;
        }

        public async Task<Job> CreateJob(int[] input)
        {
            return await _dbHandler.Insert(input);
        }

        public async Task ProcessJob(Job job)
        {
            try
            {
                _logger.LogInformation($"begin processing job {job.Id}");
                job.Status = Enum.GetName(JobStatus.InProgress);
                await _dbHandler.Update(job);
                var output = new int[job.Input.Length];
                Array.Copy(job.Input, output, job.Input.Length);
                int delay = 15;
                if (int.TryParse(_config["taskDelayInSeconds"], out int delayFromConfig))
                {
                    delay = delayFromConfig;
                }
                await General.SortArray(output, delay);
                job.Output = output;
                job.Status = Enum.GetName(JobStatus.Completed);
                job.DurationInSeconds = DateTime.Now.Subtract(job.Created).Seconds;
                await _dbHandler.Update(job);
                _logger.LogInformation($"finished processing job {job.Id} in {job.DurationInSeconds} seconds");
            }
            catch (Exception)
            {
                job.Status = Enum.GetName(JobStatus.Aborted);
                await _dbHandler.Update(job);
                throw;
            }

        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
        }
    }
}
