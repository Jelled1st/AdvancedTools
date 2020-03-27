using System.Collections.Generic;
using System;
using GXPEngine;	// Use this for Time

namespace AgentMain {

	class Alpha : Agent {
		static Random myrandom=new Random();

		public Alpha(string name) : base(name) {
		}

		public override int ChooseMove (GameBoard current, int timeLeftMS)
		{
			int ID = current.GetActivePlayer ();
			Console.WriteLine (name+": I'm playing as player {0}", ID);

			// TODO: Implement your best agent here - an improvement of the MiniMax + Monte Carlo agent.

			List<int> moves = current.GetMoves ();
			Console.WriteLine(name+": Hm let's try this move...");
			return moves [myrandom.Next (0, moves.Count)];
		}
	}
}