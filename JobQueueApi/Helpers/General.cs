using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobQueueApi.Helpers
{
    public static class General
    {
        public static async Task SortArray(int[] input, int delay)
        {
            await Task.Delay(TimeSpan.FromSeconds(delay));
            Array.Sort(input);
        }
    }
}
