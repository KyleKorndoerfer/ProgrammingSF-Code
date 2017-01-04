using System;
using System.Fabric;
using System.ServiceModel.Channels;

using Common;

using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;

namespace SimpleStoreClient
{
	class Program
	{
		static void Main( string[] args )
		{
			Uri serviceUri = new Uri( "fabric:/SimpleStoreApplication/ShoppingCartService" );

			Binding binding = WcfUtility.CreateTcpClientBinding();
			IServicePartitionResolver partitionResolver = new ServicePartitionResolver( () => new FabricClient() );

			Client shoppingClient = new Client(
					new WcfCommunicationClientFactory<IShoppingCartService>( binding, null, partitionResolver ),
					serviceUri );

			shoppingClient.AddItem( new ShoppingCartItem
			{
				ProductName = "XBOX ONE",
				UnitPrice = 329.0,
				Amount = 2
			} ).Wait();

			var list = shoppingClient.GetItems().Result;
			foreach (var item in list)
			{
				Console.WriteLine( $"{item.ProductName}: {item.UnitPrice:C2} x {item.Amount} = {item.LineTotal:C2}" );
			}

			Console.ReadKey();
		}
	}
}
