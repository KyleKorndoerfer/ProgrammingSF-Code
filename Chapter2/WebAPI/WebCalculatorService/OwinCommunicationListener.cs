using System;
using System.Fabric;
using System.Fabric.Description;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Microsoft.ServiceFabric.Services;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Owin;

namespace WebCalculatorService
{
	internal class OwinCommunicationListener : ICommunicationListener
	{
		private readonly ServiceEventSource eventSource;
		private readonly Action<IAppBuilder> startup;
		private readonly ServiceContext serviceContext;
		private readonly string endpointName;
		private readonly string appRoot;

		private IDisposable webApp;
		private string publishAddress;
		private string listeningAddress;

		public OwinCommunicationListener( Action<IAppBuilder> startup, ServiceContext serviceContext,
				ServiceEventSource eventSource, string endpointName )
			: this( startup, serviceContext, eventSource, endpointName, null )
		{
		}

		public OwinCommunicationListener( Action<IAppBuilder> startup, ServiceContext serviceContext,
			ServiceEventSource eventSource, string endpointName, string appRoot )
		{
			if (startup == null) { throw new ArgumentNullException( nameof( startup ) ); }
			if (serviceContext == null) { throw new ArgumentNullException( nameof( serviceContext ) ); }
			if (endpointName == null) { throw new ArgumentNullException( nameof( endpointName ) ); }
			if (eventSource == null) { throw new ArgumentNullException( nameof( eventSource ) ); }

			this.startup = startup;
			this.serviceContext = serviceContext;
			this.endpointName = endpointName;
			this.eventSource = eventSource;
			this.appRoot = appRoot;
		}

		#region Implementation of ICommunicationListener

		public Task<string> OpenAsync( CancellationToken cancellationToken )
		{
			EndpointResourceDescription serviceEndpoint = serviceContext.CodePackageActivationContext.GetEndpoint( endpointName );
			var protocol = serviceEndpoint.Protocol;
			int port = serviceEndpoint.Port;

			listeningAddress = string.Format(
				CultureInfo.InvariantCulture,
				"{0}://+:{1}/{2}",
				protocol,
				port,
				string.IsNullOrWhiteSpace( appRoot ) ? string.Empty : appRoot.TrimEnd( '/' ) + '/'
				);

			publishAddress = listeningAddress.Replace( "+", FabricRuntime.GetNodeContext().IPAddressOrFQDN );

			try
			{
				eventSource.Message( "Starting web server on " + listeningAddress );

				webApp = WebApp.Start( listeningAddress, appBuilder => startup.Invoke( appBuilder ) );

				eventSource.Message( "Listening on " + publishAddress );

				return Task.FromResult( publishAddress );
			}
			catch (Exception e)
			{
				eventSource.Message( "Web server failed to open endpoint {0}. {1}", endpointName, e.ToString() );

				StopWebServer();

				throw;
			}
		}

		public Task CloseAsync( CancellationToken cancellationToken )
		{
			eventSource.Message( "Closing the web server on endpoint {0}", endpointName );

			StopWebServer();

			return Task.FromResult(true);
		}

		public void Abort()
		{
			eventSource.Message( "Aborting web server on endpoint", endpointName );

			StopWebServer();
		}

		#endregion

		private void StopWebServer()
		{
			if (webApp != null)
			{
				try
				{
					webApp.Dispose();
				}
				catch (ObjectDisposedException)
				{
					// no-op
				}
			}
		}
	}
}
