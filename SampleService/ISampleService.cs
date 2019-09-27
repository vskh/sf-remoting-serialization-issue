using System.Collections.Generic;
using System.Threading.Tasks;

namespace SampleService
{
    public interface ISampleService
    {
	    Task<int> SimpleTypeShouldWorkV1V2Async(int max);
	    Task<Wrapper<int>> GenericStructShouldWorkV1V2Async(int value);
	    //Task<List<int>> SpecificCollectionTypeShouldWorkV1V2Async(int[] values);
	    Task<IEnumerable<int>> KnownCollectionInterfaceShouldWorkV21(int[] values);
    }
}
