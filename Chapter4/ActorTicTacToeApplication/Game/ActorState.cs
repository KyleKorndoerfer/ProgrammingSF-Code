using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Game
{
	[DataContract]
	public class ActorState
	{
		[DataMember]
		public int[] Board;

		[DataMember]
		public string Winner;

		[DataMember]
		public List<Tuple<long, string>> Players;

		[DataMember]
		public int NextPlayerIndex;

		[DataMember]
		public int NumberOfMoves;
	}
}
