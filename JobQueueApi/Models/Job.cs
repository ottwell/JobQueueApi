
using JobQueueApi.Enums;
using System;

namespace JobQueueApi.Models
{
    public class Job : DBItem
    {
        
        public long DurationInSeconds { get; set; }
        public string Status { get; set; }
        public int[] Input { get; set; }
        public int[] Output { get; set; }

        public Job()
        {
        }

        public Job(int[] input)
        {
            this.Id = Guid.NewGuid();
            this.Created = DateTime.Now;
            this.DurationInSeconds = 0;
            this.Input = input;
            this.Output = new int[input.Length];
            this.Status = Enum.GetName(JobStatus.Queued);
        }

    }
}
