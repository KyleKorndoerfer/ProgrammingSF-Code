As stated in the book, the code for the OWIN self-hosted communication listener
was taken from the following code sample listed below, which I have mostly replicated here in
case it changes in the future.

[https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-reliable-services-communication-webapi](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-reliable-services-communication-webapi)

The book appears to have left a few things out as well as the Service Fabric SDK
being updated to remove the `ServiceInitializationParameters` class in favor of
the `ServiceContext` base class.