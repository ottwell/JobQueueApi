using JobQueueApi.Helpers;
using System.Threading.Tasks;
using Xunit;

namespace JobQueueApiTest
{
    public class HelperMethodsTest
    {
        [Fact]
        public async Task Can_Sort_Array_Ascending()
        {
            var arr1 = new int[] { 6, 5, 4, 3, 2 };
            await General.SortArray(arr1, 0);
            Assert.Equal(arr1, new int[] { 2, 3, 4, 5, 6 });
        }
    }
}
