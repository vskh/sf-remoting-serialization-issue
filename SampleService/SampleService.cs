using System;
using System.Collections.Generic;
using System.Fabric;
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


		public Task<int> GetIntAsync(int max)
		{
			return Task.FromResult(m_random.Next(max));
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
					new ServiceRemotingMessageDispatcher(new[] {typeof(ISampleService)}, context, this),
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

			try
			{
				int value = await sampleService.GetIntAsync(100);
				Console.WriteLine($"Sample service returned '{value}'.");
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine($"Caught exception: {ex}");
			}

			// TODO: Replace the following sample code with your own logic 
			//       or remove this RunAsync override if it's not needed in your service.

			long iterations = 0;

			while (true)
			{
				cancellationToken.ThrowIfCancellationRequested();

				ServiceEventSource.Current.ServiceMessage(Context, "Working-{0}", ++iterations);

				await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
			}
		}


		private readonly Random m_random = new Random();
	}
}