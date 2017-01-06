using System;
using System.Threading.Tasks;

using Game.Interfaces;
using Player.Interfaces;

using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;

namespace TestClient
{
	class Program
	{
		private static readonly string APPLICATION_URI = "fabric:/ActorTicTacToeApplication";

		static void Main( string[] args )
		{
			var player1 = ActorProxy.Create<IPlayer>( ActorId.CreateRandom(), APPLICATION_URI );
			var player2 = ActorProxy.Create<IPlayer>( ActorId.CreateRandom(), APPLICATION_URI );
			var gameId = ActorId.CreateRandom();
			var game = ActorProxy.Create<IGame>( gameId, APPLICATION_URI );

			var joinResult1 = player1.JoinGameAsync( gameId, "Player 1" );
			var joinResult2 = player2.JoinGameAsync( gameId, "Player 2" );

			if (!joinResult1.Result || !joinResult2.Result)
			{
				Console.WriteLine( "Failed to join game." );
				Console.ReadKey();
				return;
			}

			Task.Run( () => { MakeMove( player1, gameId ); } );
			Task.Run( () => { MakeMove( player2, gameId ); } );

			var gameTask = Task.Run( () =>
			 {
				 string winner = "";
				 while (winner == "")
				 {
					 var board = game.GetGameBoardAsync().Result;
					 PrintBoard( board );
					 winner = game.GetWinnerAsync().Result;
					 Task.Delay( 1000 ).Wait();
				 }

				 Console.WriteLine( "Winner is: " + winner );
			 } );

			gameTask.Wait();
			Console.ReadKey();
		}

		#region Helpers

		private static void PrintBoard( int[] board )
		{
			Console.Clear();

			for (var i = 0; i < board.Length; i++)
			{
				string marker;

				switch (board[i])
				{
					case -1:
						marker = "X";
						break;
					case 1:
						marker = "O";
						break;
					default:
						marker = ".";
						break;
				}

				Console.Write( $" {marker} " );

				if ((i + 1) % 3 == 0)
				{
					Console.WriteLine();
				}
			}
		}

		private static async void MakeMove( IPlayer player, ActorId gameId )
		{
			Random rand = new Random();
			while (true)
			{
				var x = rand.Next( 0, 3 );
				var y = rand.Next( 0, 3 );

				await player.MakeMoveAsync( gameId, x, y );
				await Task.Delay( rand.Next( 500, 2000 ) );
			}
		}

		#endregion
	}
}

