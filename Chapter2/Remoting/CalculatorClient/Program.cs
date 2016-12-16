using System;

using CalculatorService.Interfaces;

using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace CalculatorClient
{
	internal class Program
	{
		static void Main( string[] args )
		{
			var client = ServiceProxy.Create<ICalculatorService>( new Uri( "fabric:/CalculatorApplication/CalculatorService" ) );

			var result = client.Add( 1, 2 ).GetAwaiter().GetResult();

			Console.WriteLine( result );
			Console.ReadKey();
		}
	}
}
