﻿using System.Threading.Tasks;

using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;

using Game.Interfaces;
using Player.Interfaces;


namespace Player
{
	/// <remarks>
	/// This class represents an actor.
	/// Every ActorID maps to an instance of this class.
	/// The StatePersistence attribute determines persistence and replication of actor state:
	///  - Persisted: State is written to disk and replicated.
	///  - Volatile: State is kept in memory only and replicated.
	///  - None: State is kept in memory only and not replicated.
	/// </remarks>
    [StatePersistence(StatePersistence.None)]
	internal class Player : Actor, IPlayer
	{
		private static readonly string APPLICATION_URI = "fabric:/ActorTicTacToeApplication";

		/// <summary>
		/// Initializes a new instance of Player
		/// </summary>
		/// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
		/// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
		public Player( ActorService actorService, ActorId actorId ) : base( actorService, actorId )
		{
		}

		#region Implementation of IPlayer

		public Task<bool> JoinGameAsync( ActorId gameId, string playerName )
		{
			var game = ActorProxy.Create<IGame>( gameId, APPLICATION_URI );
			return game.JoinGameAsync( Id.GetLongId(), playerName );
		}

		public Task<bool> MakeMoveAsync( ActorId gameId, int x, int y )
		{
			var game = ActorProxy.Create<IGame>( gameId, APPLICATION_URI );
			return game.MakeMoveAsync( Id.GetLongId(), x, y );
		}

		#endregion
	}
}
