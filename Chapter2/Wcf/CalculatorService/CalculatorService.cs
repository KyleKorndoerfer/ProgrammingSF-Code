using System.Collections.Generic;
using System.Fabric;
using System.ServiceModel;
using System.Threading.Tasks;

using CalculatorService.Interfaces;

using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;


namespace CalculatorService
{
	/// <summary>
	/// An instance of this class is created for each service instance by the Service Fabric runtime.
	/// </summary>
	internal sealed class CalculatorService : StatelessService, ICalculatorService
	{
		public CalculatorService( StatelessServiceContext context ) : base( context )
		{
		}

		#region StatelessService overrides

		protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
		{
            return new[]
            {
                new ServiceInstanceListener( context => new WcfCommunicationListener<ICalculatorService>(
                    serviceContext: context,
                    wcfServiceObject: this,
                    listenerBinding: WcfUtility.CreateTcpListenerBinding(),
                    endpointResourceName: "ServiceEndpoint" ) )
			};
		}

		#endregion

		#region ICalculatorService implementation

		public Task<string> Add( int a, int b )
		{
			var result = a + b;

			ServiceEventSource.Current.ServiceMessage( Context, "Received Add({0}, {1})", a, b);
			ServiceEventSource.Current.ServiceMessage( Context, "Return {0}", result );

			return Task.FromResult<string>( $"Instance {Context.InstanceId} returns: {result}" );
		}

		public Task<string> Subtract( int a, int b )
		{
			var result = a - b;

			ServiceEventSource.Current.ServiceMessage( Context, "Received Subtract({0}, {1})", a, b );
			ServiceEventSource.Current.ServiceMessage( Context, "Return {0}", result );

            return Task.FromResult<string>( $"Instance {Context.InstanceId} returns: {result}" );
        }

		#endregion
	}
}
