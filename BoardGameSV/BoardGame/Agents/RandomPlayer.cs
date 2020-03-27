using System;
using System.Collections.Generic;

class RandomPlayer : Agent {
	static Random myrandom=new Random();

	bool recognizeWin;

	public RandomPlayer(string name, bool pRecognizeWin=false) : base(name) {
		recognizeWin = pRecognizeWin;
	}

	public override int ChooseMove (GameBoard current, int timeLeftMS)
	{
		int ID = current.GetActivePlayer ();
		Console.WriteLine (name+": I'm playing as player {0}", ID);

		List<int> moves = current.GetMoves ();

		// Greedy playing: tries out all moves. If there is one that wins immediately, choose that one:
		if (recognizeWin) {
			for (int i = 0; i < moves.Count; i++) {
				GameBoard clone = current.Clone ();
				clone.MakeMove (moves [i]);
				if (clone.CheckWinner () == ID) {
					Console.WriteLine (name + ": I can win!");
					return moves [i];
				}
			}
		}

		int randomMove = myrandom.Next(0, moves.Count);

		GameBoard boardAfterMove = current.Clone();
		boardAfterMove.MakeMove(moves[randomMove]);
		Console.WriteLine("\nWinnable scenerios: ");
		PrintWinSceneriosInMovesRecursivelly(1, boardAfterMove);
		Console.WriteLine("\n");

		// ...otherwise, choose a random move:
		return moves [randomMove];
	}

	public void PrintWinSceneriosInMovesRecursivelly(int moves, GameBoard board)
	{
		int winSceneriosFor1 = 0; // 1
		int winSceneriosForN1 = 0; // -1

		Console.WriteLine("Getting win scenerios recursivelly");

		winSceneriosFor1 = getWinSceneriosForPlayerInMovesRecursivelly(1, moves, board);
		winSceneriosForN1 = getWinSceneriosForPlayerInMovesRecursivelly(-1, moves, board);

		Console.WriteLine("The win scenerios for +1 are: " + winSceneriosFor1 + "\n\t\tand for -1: " + winSceneriosForN1);
	}

	private int getWinSceneriosForPlayerInMovesRecursivelly(int player, int moves, GameBoard board, int depth = 0)
	{
		string readabilityTabs = "";
		for(int i = 0; i < depth; ++i)
		{
			readabilityTabs += "\t";
		}

		int winScenerios = 0;
		List<int> boardMoves = board.GetMoves();
		Console.WriteLine(readabilityTabs + boardMoves.Count + " available for clone");
		for(int i = 0; i < boardMoves.Count; ++i)
		{
			Console.WriteLine(readabilityTabs + "currently checking move " + i);
			GameBoard clone = board.Clone();
			clone.MakeMove(boardMoves[i]);
			Console.WriteLine(readabilityTabs + "player " + clone.CheckWinner() + " will win");
			if(clone.CheckWinner() == player)
			{
				++winScenerios;
			}
			else if(clone.MaxMovesLeft() != 0 && moves != 0)
			{
				// no one has won and it is not a draw yet
				// there are more moves left to do
				winScenerios += getWinSceneriosForPlayerInMovesRecursivelly(player, moves - 1, clone, depth+1);
			}
		}

		return winScenerios;
	}
}
