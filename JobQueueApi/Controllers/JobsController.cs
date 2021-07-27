using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JobQueueApi.Models;
using Microsoft.Extensions.Logging;
using JobQueueApi.Interfaces;
using JobQueueApi.Enums;


namespace JobQueueApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {

        private ILogger _logger;

        private IRequestProcessor _proccessor;

        private IBackgroundTaskQueue _queue;


        public JobsController(IRequestProcessor processor, ILoggerFactory logFactory, IBackgroundTaskQueue queue)
        {
            _proccessor = processor;
            _logger = logFactory.CreateLogger<JobsController>();
            _queue = queue;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Job>>> GetJobs()
        {
            try
            {
                return await _proccessor.GetAll();
            }
            catch (Exception e)
            {
                _logger.LogError($"an error occured while retrieving jobs from DB. Message: {e.Message}. stack: {e.StackTrace}");
                return StatusCode(500);
            }

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<JobResult>> GetJob(Guid id)
        {
            try
            {
                var job = await _proccessor.GetById(id);
                if (job == null)
                {
                    _logger.LogInformation($"found no jobs in DB that match the given id: {id}.");
                    return NotFound();
                }
                var result = new JobResult()
                {
                    Job = job,
                    CurrentPlaceInQueue = -1
                };
                if (Enum.Parse<JobStatus>(job.Status) == JobStatus.Queued)
                {
                    result.CurrentPlaceInQueue = _queue.GetCurrentItemQueuePosition(job.Id);
                }
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"an error occured while retrieving job from DB. id: {id}. Message: {e.Message}. stack: {e.StackTrace}");
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<JobResult>> PostJob([FromBody] int[] input)
        {

            try
            {
                var job = await _proccessor.CreateJob(input);
                var workItem = new WorkItem
                {
                    Id = job.Id,
                    Action = async () =>
                    {
                        try
                        {
                            await _proccessor.ProcessJob(job);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError($"an error occured while processing job {job.Id}. Message: {e.Message}. stack: {e.StackTrace}");
                        }

                    }
                };
                _queue.QueueBackgroundWorkItem(workItem);

                return CreatedAtAction(nameof(GetJob), new { id = job.Id }, new JobResult() { Job = job, CurrentPlaceInQueue = _queue.GetCurrentQueueLength() }); ;

            }
            catch (Exception e)
            {
                _logger.LogError($"an error occured while saving new job to DB. Message: {e.Message}. stack: {e.StackTrace}");
                return StatusCode(500);
            }

        }


        [HttpPut("{id}")]
        public async Task<ActionResult<JobResult>> UpdateJob(Guid id, [FromBody] int[] input)
        {

            try
            {
                var job = await _proccessor.GetById(id);
                if (job == null)
                {
                    _logger.LogInformation($"found no jobs in DB that match the given id: {id}.");
                    return NotFound();
                }
                job.Input = input;
                job.Output = new int[input.Length];
                var workItem = new WorkItem
                {
                    Id = job.Id,
                    Action = async () =>
                    {
                        try
                        {
                            await _proccessor.ProcessJob(job);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError($"an error occured while processing job {job.Id}. Message: {e.Message}. stack: {e.StackTrace}");
                        }

                    }
                };
                _queue.QueueBackgroundWorkItem(workItem);
                return AcceptedAtAction(nameof(GetJob), new { id = job.Id }, new JobResult() { Job = job, CurrentPlaceInQueue = _queue.GetCurrentQueueLength() }); ;

            }
            catch (Exception e)
            {
                _logger.LogError($"an error occured while saving new job to DB. Message: {e.Message}. stack: {e.StackTrace}");
                return StatusCode(500);
            }

        }


    }
}
