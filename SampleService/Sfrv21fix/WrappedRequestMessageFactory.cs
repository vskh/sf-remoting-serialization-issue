using Microsoft.ServiceFabric.Services.Remoting.V2;

namespace SampleService.Sfrv21fix
{
	public class WrappedRequestMessageFactory : IServiceRemotingMessageBodyFactory
	{
		public IServiceRemotingRequestMessageBody CreateRequest(
			string interfaceName,
			string methodName,
			int numberOfParameters,
			object wrappedRequestObject)
		{
			WrappedRemotingMessageBody remotingMessageBody = new WrappedRemotingMessageBody();
			remotingMessageBody.Value = wrappedRequestObject;
			return (IServiceRemotingRequestMessageBody)remotingMessageBody;
		}

		public IServiceRemotingResponseMessageBody CreateResponse(
			string interfaceName,
			string methodName,
			object wrappedResponseObject)
		{
			WrappedRemotingMessageBody remotingMessageBody = new WrappedRemotingMessageBody();
			remotingMessageBody.Value = wrappedResponseObject;
			return (IServiceRemotingResponseMessageBody)remotingMessageBody;
		}
	}
}
