using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SampleService.Sfrv21fix;

namespace SampleService
{
	/// <summary>
	/// An instance of this class is created for each service instance by the Service Fabric runtime.
	/// </summary>
	internal sealed class SampleService : StatelessService, ISampleService
	{
		public SampleService(StatelessServiceContext context)
			: base(context)
		{
		}


		public Task<int> SimpleTypeShouldWorkV1V2Async(int value)
		{
			return Task.FromResult(value);
		}


		public Task<Wrapper<int>> GenericStructShouldWorkV1V2Async(int value)
		{
			Wrapper<int> w = value;

			return Task.FromResult(w);
		}


		public Task<IEnumerable<int>> KnownCollectionInterfaceShouldWorkV21(int[] values)
		{
			IEnumerable<int> l = values.ToList();

			return Task.FromResult(l);
		}


		public Task<List<int>> SpecificCollectionTypeShouldWorkV1V2Async(int[] values)
		{
			List<int> l = values.ToList();

			return Task.FromResult(l);
		}


		/// <summary>
		/// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
		/// </summary>
		/// <returns>A collection of listeners.</returns>
		protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
		{
			yield return new ServiceInstanceListener(context =>
				new FabricTransportServiceRemotingListener(
					context,
					new ServiceRemotingMessageDispatcher(
						new[] {typeof(ISampleService)},
						context, 
						this,
						new WrappedRequestMessageFactory()),
					new FabricTransportRemotingListenerSettings
					{
						UseWrappedMessage = true
					}));
		}


		/// <summary>
		/// This is the main entry point for your service instance.
		/// </summary>
		/// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
		protected override async Task RunAsync(CancellationToken cancellationToken)
		{
			ServiceProxyFactory factory = new ServiceProxyFactory(handler =>
				new FabricTransportServiceRemotingClientFactory(new FabricTransportRemotingSettings
				{
					UseWrappedMessage = true
				}));

			ISampleService sampleService = factory.CreateNonIServiceProxy<ISampleService>(
				new Uri("fabric:/SFRemotingSerializationIssue/SampleService"));

			while (true)
			{
				cancellationToken.ThrowIfCancellationRequested();

				try
				{
					int simpleValue = await sampleService.SimpleTypeShouldWorkV1V2Async(100);
					Console.WriteLine($"SimpleType works: '{simpleValue}'.");

					int wrappedValue = await sampleService.GenericStructShouldWorkV1V2Async(100);
					Console.WriteLine($"GenericType works: '{wrappedValue}'.");

					// commented out as V2 cannot generate data contract if there are two methods with different
					// types that resolve to same contract name
					//List<int> collectionValue = await sampleService.SpecificCollectionTypeShouldWorkV1V2Async(new[] { 1, 2, 3 });
					//Console.WriteLine($"CollectionType works: '{string.Join(", ", collectionValue)}'.");

					IEnumerable<int> enumerableValue =
						await sampleService.KnownCollectionInterfaceShouldWorkV21(new[] {1, 2, 3});
					Console.WriteLine($"CollectionInterface works: '{string.Join(", ", enumerableValue)}'.");
				}
				catch (Exception ex)
				{
					Console.Error.WriteLine($"Caught exception: {ex}");
				}

				await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
			}
		}
	}
}