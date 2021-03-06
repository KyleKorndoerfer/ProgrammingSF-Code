﻿using System;
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

			for (int customerId = 0; customerId < 10; customerId++)
			{
				Client shoppingClient = new Client(
						new WcfCommunicationClientFactory<IShoppingCartService>( binding, null, partitionResolver ),
						serviceUri,
						customerId );

				shoppingClient.AddItem( new ShoppingCartItem
				{
					ProductName = "XBOX ONE",
					UnitPrice = 329.0,
					Amount = 2
				} ).Wait();

				PrintPartition( shoppingClient );

				var list = shoppingClient.GetItems().Result;
				foreach (var item in list)
				{
					Console.WriteLine( $"{item.ProductName}: {item.UnitPrice:C2} x {item.Amount} = {item.LineTotal:C2}" );
				}
			}

			Console.ReadKey();
		}

		private static void PrintPartition( Client client )
		{
			ResolvedServicePartition partition;
			if (client.TryGetLastResolvedServicePartition( out partition ))
			{
				Console.WriteLine( "Partition ID: " + partition.Info.Id );
			}
		}
	}
}
