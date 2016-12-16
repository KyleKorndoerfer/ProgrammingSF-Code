using System.Collections.Generic;
using System.Fabric;
using System.Threading.Tasks;

using CalculatorService.Interfaces;

using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
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
				new ServiceInstanceListener( context => this.CreateServiceRemotingListener(context) )
			};
		}

		#endregion

		#region ICalculatorService implementation

		public Task<int> Add( int a, int b )
		{
			var result = a + b;

			ServiceEventSource.Current.ServiceMessage( Context, "Received Add({0}, {1})", a, b);
			ServiceEventSource.Current.ServiceMessage( Context, "Return {0}", result );

			return Task.FromResult<int>( result );
		}

		public Task<int> Subtract( int a, int b )
		{
			var result = a - b;

			ServiceEventSource.Current.ServiceMessage( Context, "Received Subtract({0}, {1})", a, b );
			ServiceEventSource.Current.ServiceMessage( Context, "Return {0}", result );

			return Task.FromResult<int>( result );
		}

		#endregion
	}
}
