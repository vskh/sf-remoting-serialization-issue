using System.Threading.Tasks;

namespace SampleService
{
    public interface ISampleService
    {
	    Task<int> GetIntAsync(int max);
    }
}
