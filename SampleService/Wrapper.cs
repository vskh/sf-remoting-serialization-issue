using System.Runtime.Serialization;

namespace SampleService
{
	public struct Wrapper<T>
    {
	    public T Value { get; set; }
		
        public static implicit operator Wrapper<T>(T value) => new Wrapper<T>{ Value = value };
		public static implicit operator T(Wrapper<T> wrapped) => wrapped.Value;
    }
}
