using System;
using System.Collections.Generic;

class MonteCarlo : Agent {
	static Random myrandom=new Random();

	int sampleSize;
	bool greedyRandom;

	public MonteCarlo(string name, int pSampleSize=25, bool pGreedyRandom = true) : base(name) {
		sampleSize = pSampleSize;
		greedyRandom = pGreedyRandom;
	}

	public override int ChooseMove (GameBoard current, int timeLeftMS)
	{
		int ID = current.GetActivePlayer ();
		Console.WriteLine (name+": I'm playing as player {0}", ID);

		// TODO: Implement a Monte Carlo agent here, instead of the dumb algorithm below: 
		// For every possible move, play [sampleSize] many random games on the board that you
		// get after making that move, and choose the move with the highest win percentage.

		List<int> moves = current.GetMoves();

		List<int> wins = new List<int>();
		List<int> losses = new List<int>();
		List<float> scores = new List<float>();
		int bestIndex = 0;
		Console.WriteLine("\nconsidering all movements::\n------------------------------");
		Console.WriteLine("Current active player: " + current.GetActivePlayer());
		for (int i = 0; i < moves.Count; ++i)
		{
			wins.Add(0);
			losses.Add(0);
			for (int s = 0; s < sampleSize; ++s)
			{
				GameBoard afterMove = current.Clone();
				afterMove.MakeMove(moves[i]);
				//Console.WriteLine("playing game number " + s + " amount of wins: " + wins[i]);
				int randomWinner;
				if (greedyRandom) randomWinner = greedyRandomPlay(afterMove);
				else randomWinner = randomPlay(afterMove);
				if (randomWinner == ID)
				{
					++wins[i];
				}
				else if(randomWinner == -ID)
				{
					++losses[i];
				}
			}
			//Console.WriteLine("wins: " + wins[i] + " : " + moveWins);
			//if (wins[i] > wins[bestIndex]) bestIndex = i;
			//else if(wins[i] == wins[bestIndex])
			//{
			//	if (losses[i] < losses[bestIndex]) bestIndex = i; 
			//}
			float score = (wins[i] - losses[i]) / (float)sampleSize;
			scores.Add(score);
			if (scores[i] > scores[bestIndex]) bestIndex = i;
		}

		//Console.WriteLine(name+": Hm let's try this move...");
		Console.WriteLine(name + ": played " + moves.Count + "*" + sampleSize + " games");
		string output = "";
		for(int i = 0; i < wins.Count; ++i)
		{
			output += "\tmove[" + i + "]: " + moves[i] + " resulted in " + wins[i] + " wins and " + losses[i] + " losses, score of " + scores[i] + "\n" ; 
		}
		output += "\t\tbest index is " + bestIndex + " with move: " + moves[bestIndex];
		Console.WriteLine(output);

		return moves [bestIndex];
	}

	private int randomPlay(GameBoard board)
	{
		//	int winner = 0;
		//	int loops = 0;
		//	while (true)
		//	{
		//		if (board.CheckWinner() != 0 || board.MaxMovesLeft() == 0)
		//		{
		//			winner = board.CheckWinner();
		//			break;
		//		}

		//		List<int> moves = board.GetMoves();
		//		int random = myrandom.Next(0, moves.Count);
		//		board.MakeMove(moves[random]);
		//		++loops;

		//		Console.WriteLine("winner " + board.CheckWinner() + " moves left: " + board.MaxMovesLeft());
		//	}
		//	Console.WriteLine("========================> random game finished after {0} loops!", loops);
		//	return winner;

		int winner = 0;
		int loops = 0;
		while (true)
		{
			if (board.CheckWinner() != 0 || board.MaxMovesLeft() == 0)
			{
				winner = board.CheckWinner();
				break;
			}

			List<int> moves = board.GetMoves();
			int random = myrandom.Next(0, moves.Count);
			board.MakeMove(moves[random]);
			++loops;
		}
		Console.WriteLine("========================> random game finished after {0} loops, winner {1}", loops, winner);
		return winner;
	}

	private int greedyRandomPlay(GameBoard board)
	{
		int winner = 0;
		int loops = 0;
		while (true)
		{
			if (board.CheckWinner() != 0 || board.MaxMovesLeft() == 0)
			{
				winner = board.CheckWinner();
				break;
			}

			List<int> moves = board.GetMoves();
			//recognize win - if the next move can result in a win, do that move
			for(int m = 0; m < moves.Count; ++m)
			{
				GameBoard clone = board.Clone();
				clone.MakeMove(moves[m]);
				if(clone.CheckWinner() == -clone.GetActivePlayer()) //check if the winner is the one who just made the move
				{
					winner = clone.CheckWinner();
					Console.WriteLine("found a winning move for {0}", winner);
					break;
				}
			}
			if (winner != 0) break;
			int random = myrandom.Next(0, moves.Count);
			board.MakeMove(moves[random]);
			++loops;
		}
		Console.WriteLine("========================> random game finished after {0} loops, winner {1}", loops, winner);
		return winner;
	}
}
