using System;
using System.ServiceModel;
using System.Threading.Tasks;

using CalculatorService.Interfaces;

using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;

namespace CalculatorClient
{
    internal class Program
    {
        static void Main( string[] args )
        {
            var iteration = 0;

            while ( true )
            {
                var client = new ServiceClient( new Uri( "fabric:/CalculatorApplication/CalculatorService" ) );
                var result = client.Add( 1, 2 ).GetAwaiter().GetResult();

                Console.WriteLine( $"({++iteration:D4}) {result}" );

                System.Threading.Thread.Sleep( TimeSpan.FromSeconds( 5 ) );
            }
        }
    }

    public class ServiceClient : ServicePartitionClient<WcfCommunicationClient<ICalculatorService>>, ICalculatorService
    {
        private static ICommunicationClientFactory<WcfCommunicationClient<ICalculatorService>> communicationClientFactory;

        static ServiceClient()
        {
            communicationClientFactory = new WcfCommunicationClientFactory<ICalculatorService>( clientBinding: WcfUtility.CreateTcpClientBinding() );
        }

        public ServiceClient( Uri serviceUri ) : this( serviceUri, ServicePartitionKey.Singleton )
        {
        }

        public ServiceClient( Uri serviceUri, ServicePartitionKey partitionKey ) : base( communicationClientFactory, serviceUri, partitionKey )
        {
        }

        #region ICalculatorService

        public Task<string> Add( int a, int b )
        {
            return this.InvokeWithRetry( c => c.Channel.Add( a, b ) );
        }

        public Task<string> Subtract( int a, int b )
        {
            return this.InvokeWithRetry( c => c.Channel.Subtract( a, b ) );
        }

        #endregion
    }
}
