using System;
using System.Runtime.Serialization;
using Microsoft.ServiceFabric.Services.Remoting.V2;

namespace SampleService.Sfrv21fix
{
	[DataContract(Name = "WrappedMsgBody", Namespace = "urn:ServiceFabric.Communication")]
	public class WrappedRemotingMessageBody : WrappedMessage, IServiceRemotingRequestMessageBody, IServiceRemotingResponseMessageBody
	{
		public void SetParameter(int position, string parameName, object parameter)
		{
			throw new NotImplementedException();
		}

		public object GetParameter(int position, string parameName, Type paramType)
		{
			throw new NotImplementedException();
		}

		public void Set(object response)
		{
			throw new NotImplementedException();
		}

		public object Get(Type paramType)
		{
			throw new NotImplementedException();
		}
	}
}
