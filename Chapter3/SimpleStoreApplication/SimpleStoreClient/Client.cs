using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Common;

using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;

namespace SimpleStoreClient
{
	public class Client : ServicePartitionClient<WcfCommunicationClient<IShoppingCartService>>, IShoppingCartService
	{
		public Client( WcfCommunicationClientFactory<IShoppingCartService> clientFactory, Uri serviceUri )
			: base( clientFactory, serviceUri, new ServicePartitionKey(1) )
		{
		}

		#region Implementation of IShoppingCartService

		public Task AddItem( ShoppingCartItem item )
		{
			return InvokeWithRetryAsync( client => client.Channel.AddItem( item ) );
		}

		public Task DeleteItem( string productName )
		{
			return InvokeWithRetryAsync( client => client.Channel.DeleteItem( productName ) );
		}

		public Task<List<ShoppingCartItem>> GetItems()
		{
			return InvokeWithRetryAsync( client => client.Channel.GetItems() );
		}

		#endregion
	}
}
