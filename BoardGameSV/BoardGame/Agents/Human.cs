using System;

class Human : Agent {
	public Human (string name) : base (name) {
	}

	// In this (graphic) version of the program, the following code is actually not used!
	public override int ChooseMove(GameBoard current, int timeLeftMS) {
		int done = -1;
		int move = 0;
		while (done==-1) {
			Console.WriteLine (current.ToString ());
			Console.Write ("Available moves: ");
			foreach (int col in current.GetMoves())
				Console.Write (col + " ");
			Console.WriteLine ("\nPlayer "+current.Symbol(current.GetActivePlayer())+", make your move!");
			try {
				move = int.Parse (Console.ReadLine ());
				done = current.MakeMove (move);	// just for trying whether it's a legal move - this is the cloned board
			} catch {
				Console.WriteLine ("Try again...");
			}
		}
		return move;
	}
}
