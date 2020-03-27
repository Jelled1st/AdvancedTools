using System;
using System.Collections.Generic;

class Greedy : Agent {
	static Random myrandom=new Random();

	public Greedy(string name) : base(name) {
	}

	public override int ChooseMove (GameBoard current, int timeLeftMS)
	{
		int ID = current.GetActivePlayer ();
		Console.WriteLine (name+": I'm playing as player {0}", ID);

		if (current is OthelloBoard)
			return ChooseOthelloMove ((OthelloBoard)current);
		if (current is ConnectFourBoard || current is PentagoBoard || current is GomokuBoard)
			return ChooseSequenceBoardMove ((SquareBoard)current);

		// ...otherwise, choose a random move:
		Console.WriteLine(name+": Hm I don't know this game let's try this move...");
		List<int> moves = current.GetMoves ();
		return moves [myrandom.Next (0, moves.Count)];
	}

	int ChooseOthelloMove(OthelloBoard current) {		
		List<int> moves = current.GetMoves ();

		int ID = current.GetActivePlayer ();
		int bestscore = -64;	// worst possible score
		int bestmoveindex=0;
		for (int i = 0; i < moves.Count; i++) {
			OthelloBoard clone = (OthelloBoard)current.Clone ();
			clone.MakeMove (moves [i]);
			int newscore = clone.CountStones () * ID;
			// Grabbing corners = always good:
			if (moves [i] == 0 || moves [i] == 7 || moves [i] == 56 || moves [i] == 63)
				newscore += 50;
			if (newscore>bestscore) {
				bestscore = newscore;
				bestmoveindex = i;
			}
		}
		return moves[bestmoveindex];
	}

	int ChooseSequenceBoardMove(SquareBoard current) {
		List<int> moves = current.GetMoves ();
		int ID = current.GetActivePlayer ();
		double bestscore = -current._width*current._height * 25;	// worst possible score (?)
		int bestmoveindex=0;
		for (int i = 0; i < moves.Count; i++) {
			GameBoard clone = current.Clone ();
			clone.MakeMove (moves [i]);
			double newscore = CountSequenceLengths((SquareBoard)clone)*ID;
			// In case of tie, prefer central moves:
			if (current is ConnectFourBoard)
				newscore += (3 - Math.Abs (moves [i] - 3)) * 0.1;
			else {				
				newscore += (current._width / 2 - Math.Abs (moves [i] % current._width - current._width / 2) +
							 (current._height / 2 - Math.Abs (moves [i] / current._width - current._height / 2))) * 0.1;
			}
			if (newscore>bestscore) {
				bestscore = newscore;
				bestmoveindex = i;
			}
		}
		return moves[bestmoveindex];
	}

	public double CountSequenceLengths(SquareBoard current, float power=2) {
		double total = 0;
		for (int row = 0; row < current._height; row++)
			for (int col = 0; col < current._width; col++) {
				if (current[row,col]!=0) {
					total+=Math.Pow(current.countaround(row,col,1,0),power)*current[row,col];	// row
					total+=Math.Pow(current.countaround(row,col,0,1),power)*current[row,col];	// col
					total+=Math.Pow(current.countaround(row,col,1,1),power)*current[row,col];	// down diag
					total+=Math.Pow(current.countaround(row,col,1,-1),power)*current[row,col];	// up diag
				}
			}
		return total;
	}
}
