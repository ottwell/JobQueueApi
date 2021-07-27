using JobQueueApi.Controllers;
using JobQueueApi.Enums;
using JobQueueApi.Interfaces;
using JobQueueApi.Models;
using JobQueueApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace JobQueueApiTest
{
    public class JobsControllerTest
    {
        private IConfiguration _config;

        private IDBHandler _handler;

        private IRequestProcessor _processor;

        private IBackgroundTaskQueue _queue;

        private ILoggerFactory _loggerFactory;

        private JobsController _controller;

        private BackgroundWorker _backgroundWorker;

        private readonly ITestOutputHelper _output;

        public IConfiguration Configuration
        {
            get
            {
                if (_config == null)
                {
                    var builder = new ConfigurationBuilder().AddJsonFile($"testsettings.json", optional: false);
                    _config = builder.Build();
                }

                return _config;
            }
        }
        public JobsControllerTest(ITestOutputHelper output)
        {
            _output = output;
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton(Configuration);
            services.AddSingleton<IDBHandler, FakeDBHandler>();
            services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
            services.AddSingleton<IRequestProcessor, RequestProcessor>();
            services.AddHostedService<BackgroundWorker>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            var serviceProvider = services.BuildServiceProvider();
            _loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            _handler = serviceProvider.GetService<IDBHandler>();
            _processor = serviceProvider.GetService<IRequestProcessor>();
            _queue = serviceProvider.GetService<IBackgroundTaskQueue>();
            _backgroundWorker = serviceProvider.GetService<IHostedService>() as BackgroundWorker;
            _backgroundWorker.StartAsync(CancellationToken.None).Wait();
            _controller = new JobsController(_processor, _loggerFactory, _queue);
        }


        [Fact]
        public async Task Can_get_jobs()
        {
            var jobs = await _controller.GetJobs();
            Assert.Equal(4, jobs.Value.Count());
        }

        [Fact]
        public async Task Can_get_job_by_id()
        {
            var nonExistingJob = await _controller.GetJob(Guid.NewGuid());
            Assert.IsAssignableFrom<NotFoundResult>(nonExistingJob.Result);
            var existingJob = await _controller.GetJob(Guid.Parse("523f20c9-5def-491f-b5ff-3f3d2f4fa80b"));
            Assert.IsAssignableFrom<JobResult>(existingJob.Value);
        }

        [Fact]
        public async Task Can_post_job()
        {
            var unsorted = new int[] { 4, 6, 1, 2, -1000000 };
            var result = await _controller.PostJob(unsorted);
            Assert.IsAssignableFrom<CreatedAtActionResult>(result.Result);
            await Task.Delay(TimeSpan.FromSeconds(int.Parse(_config["taskDelayInSeconds"])));
            var jobId = ((JobResult)((CreatedAtActionResult)result.Result).Value).Job.Id;
            var job = await _controller.GetJob(jobId);
            Assert.Equal(job.Value.Job.Status, Enum.GetName(JobStatus.Completed));

        }
    }

}
