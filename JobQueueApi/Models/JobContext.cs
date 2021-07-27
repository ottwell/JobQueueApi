using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace JobQueueApi.Models
{
    public class JobContext : DbContext
    {
        public JobContext(DbContextOptions<JobContext> options) : base(options) { }

        public DbSet<Job> Jobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Job>()
            .Property(e => e.Input)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.None).Select(n => Convert.ToInt32(n)).ToArray());

            modelBuilder.Entity<Job>()
            .Property(e => e.Output)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.None).Select(n => Convert.ToInt32(n)).ToArray());

            
        }



    }
}
