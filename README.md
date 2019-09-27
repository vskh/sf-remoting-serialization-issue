# ServiceFabric Remoting V2_1 serialization issue (package v3.4.664) 
Minimal repro project to demonstrate the issue https://github.com/Azure/service-fabric-issues/issues/1578

With current latest SF Remoting library v3.4.664 remoting V2_1 does not work if setup according to [this](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-reliable-services-communication-remoting#use-the-remoting-v2-interface-compatible-stack) instructions.

As example in master branch of this repo shows, even in cases that work with V1 or V2 it throws the following exception:
```
Type 'Microsoft.ServiceFabric.Services.Remoting.V2.ServiceRemotingResponseMessageBody' with data contract name 'msgResponse:urn:ServiceFabric.Communication' is not expected. Consider using a DataContractResolver if you are using DataContractSerializer or add any types not known statically to the list of known types - for example, by using the KnownTypeAttribute attribute or by adding them to the list of known types passed to the serializer.

   at Microsoft.ServiceFabric.Services.Communication.Client.ServicePartitionClient`1.<InvokeWithRetryAsync>d__23`1.MoveNext()
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at Microsoft.ServiceFabric.Services.Remoting.V2.Client.ServiceRemotingPartitionClient.<InvokeAsync>d__2.MoveNext()
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at Microsoft.ServiceFabric.Services.Remoting.Builder.ProxyBase.<InvokeAsyncV2>d__21.MoveNext()
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at Microsoft.ServiceFabric.Services.Remoting.Builder.ProxyBase.<ContinueWithResultV2>d__20`1.MoveNext()
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter`1.GetResult()
   at SampleService.SampleService.<RunAsync>d__6.MoveNext() in C:\Users\vadym\source\repos\SFRemotingSerializationIssue\SampleService\SampleService.cs:line 97
```

Debugging revealed that this happens because ServiceRemotingMessageDispatcher class does not honor V2_1 setup (UseWrappedMessage = true).

# Workaround
It is possible to make it work without patching SF Remoting library (if you are in hurry).
[v21fix](https://github.com/vskh/sf-remoting-serialization-issue/tree/v21fix) branch of this repository shows how.

It involves copying [WrappedRemotingMessageBody](https://github.com/vskh/sf-remoting-serialization-issue/blob/v21fix/SampleService/Sfrv21fix/WrappedRemotingMessageBody.cs) 
and [WrappedRequestMessageFactory](https://github.com/vskh/sf-remoting-serialization-issue/blob/v21fix/SampleService/Sfrv21fix/WrappedRequestMessageFactory.cs)
from [SF Services and Actors sources](https://github.com/microsoft/service-fabric-services-and-actors-dotnet/tree/b19956611ca2022c64551ccca54c50277e18422a/src/Microsoft.ServiceFabric.Services.Remoting/V2) 
(because they were made internal) and using the factory producing wrapped message bodies explicitly when creating a listener.
