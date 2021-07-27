using JobQueueApi.Interfaces;
using JobQueueApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobQueueApiTest
{
    class FakeDBHandler : IDBHandler
    {
        private readonly JobContext _context;

        public FakeDBHandler()
        {
            var contextOptions = new DbContextOptionsBuilder<JobContext>()
             .UseInMemoryDatabase(databaseName: "Test")
             .Options;
            _context = new JobContext(contextOptions);
            Seed();
        }

        private void Seed()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            var job1 = new Job(new int[] { 9, 8, 7, 6, 5, 4 });
            job1.Id = Guid.Parse("523f20c9-5def-491f-b5ff-3f3d2f4fa80b");
            var job2 = new Job(new int[] { 10, 1, -6684, 8, 6 });
            var job3 = new Job(new int[] { 3 });
            var job4 = new Job(new int[] { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 });
            _context.AddRange(job1, job2, job3, job4);
            _context.SaveChanges();
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
            await _context.SaveChangesAsync();
            return job;
        }

        public async Task Update(Job job)
        {
            _context.Entry(job).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
