using System;
using System.Collections.Generic;

class MiniMax : Agent {
	static Random myrandom=new Random();

	private int _searchDepth = -1;

	//if true it will use score > currentBestScore
	//else if false it will use >= currentBestScore
	private bool _onlyBetterScore;

	public MiniMax(string name, int pSearchDepth = -1, bool pOnlyBetterScore = true) : base(name) {
		_searchDepth = pSearchDepth;
		_onlyBetterScore = pOnlyBetterScore;
	}

	public override int ChooseMove (GameBoard current, int timeLeftMS)
	{
		int ID = current.GetActivePlayer ();
		Console.WriteLine (name+": I'm playing as player {0}", ID);

		// TODO: Implement a recursive MiniMax algorithm here, using a Monte Carlo evaluation function, instead of the dumb algorithm below.

		//if(current.isOpeningMove())
		//{
		//	Console.WriteLine("do opening move");
		//	return current.GetBestOpeningMove();
		//}

		int bestMove = 0;
		int bestValue = 0;

		List<int> moves = current.GetMoves();
		if (ID == -1)
		{
			bestValue = 200;
			for (int m = 0; m < moves.Count; ++m)
			{
				GameBoard clone = current.Clone();
				clone.MakeMove(moves[m]);
				//value is actually the winner of the game
				int score = getMove(clone, 0);

				if(_onlyBetterScore && score <= bestValue)
				{
					bestValue = score;
					bestMove = m;
				}
				else if(!_onlyBetterScore && score < bestValue)
				{
					bestValue = score;
					bestMove = m;
				}
			}
		}
		else
		{
			bestValue = -200;
			for (int m = 0; m < moves.Count; ++m)
			{
				GameBoard clone = current.Clone();
				clone.MakeMove(moves[m]);
				//value is actually the winner of the game
				int score = getMove(clone, 0);

				if (_onlyBetterScore && score >= bestValue)
				{
					bestValue = score;
					bestMove = m;
				}
				else if (!_onlyBetterScore && score > bestValue)
				{
					bestValue = score;
					bestMove = m;
				}
			}
		}
		
		//Console.WriteLine(name+": Hm let's try this move...");
		return moves[bestMove];
	}

	private int getMove(GameBoard board, int depth)
	{
		int winner = board.CheckWinner();
		if (depth == _searchDepth || winner != 0 || board.MaxMovesLeft() == 0)
		{
			return winner;
		}

		List<int> moves = board.GetMoves();
		int bestMove = 0;
		int bestValue = 0;

		//if-statement first, because otherwise it would have to execute the if-statement as many times as the loop loops
		if (board.GetActivePlayer() == -1)
		{
			bestValue = 200;
			for (int m = 0; m < moves.Count; ++m)
			{
				GameBoard clone = board.Clone();
				clone.MakeMove(moves[m]);
				//value is actually the winner of the game
				int score = getMove(clone, depth + 1);

				if (_onlyBetterScore && score < bestValue)
				{
					bestValue = score;
					bestMove = m;
				}
				else if (!_onlyBetterScore && score <= bestValue)
				{
					bestValue = score;
					bestMove = m;
				}
			}
		}
		else
		{
			bestValue = -200;
			for (int m = 0; m < moves.Count; ++m)
			{
				GameBoard clone = board.Clone();
				clone.MakeMove(moves[m]);
				//value is actually the winner of the game
				int score = getMove(clone, depth + 1);

				if (_onlyBetterScore && score > bestValue)
				{
					bestValue = score;
					bestMove = m;
				}
				else if (!_onlyBetterScore && score >= bestValue)
				{
					bestValue = score;
					bestMove = m;
				}
			}
		}
		return bestValue;
	}

	private int getMoveOld(GameBoard board, int depth, out int requiredDepth)
	{
		string debugTabs = Helper.multiply("\t", depth);
		List<int> moves = board.GetMoves();
		Dictionary<int, int> moveValues = new Dictionary<int, int>();
		Dictionary<int, int> moveDepths = new Dictionary<int, int>();

		Console.WriteLine(debugTabs + "found {0} possible moves", moves.Count);

		int bestMove = 0;
		for(int i = 0; i < moves.Count; ++i)
		{
			Console.WriteLine(debugTabs + "applying move {0}", i);
			GameBoard clone = board.Clone();
			clone.MakeMove(moves[i]);
			int winner = clone.CheckWinner();
			if (winner != 0 || clone.MaxMovesLeft() == 0)
			{
				//reached end of try
				//either there is a winner or there are no more moves left
				Console.WriteLine(debugTabs + "found end of tree, winner: {0}", winner);
				moveValues.Add(i, winner);
			}
			else
			{
				Console.WriteLine(debugTabs + "no winner found, going deeper V");
				int moveDepth;
				moveValues.Add(i, getMoveOld(clone, depth + 1, out moveDepth));
			}

			Console.WriteLine(debugTabs + "move value: {0}", moveValues[i]);

			//min and max are switched here because once the board made it's move the active player switched
			if(board.GetActivePlayer() == 1)
			{
				if (moveValues[i] > moveValues[bestMove])
				{
					Console.WriteLine(debugTabs + "this move was the best yet");
					bestMove = i;
				}
			}
			else if(board.GetActivePlayer() == -1)
			{
				if (moveValues[i] < moveValues[bestMove])
				{
					Console.WriteLine(debugTabs + "this move was the best yet");
					bestMove = i;
				}
			}
		}

		if (true)
		{
			for (int i = 0; i < moves.Count; ++i)
			{
				Console.WriteLine(debugTabs + "Found move[{0}] with value {1}", i, moveValues[i]);
			}
			Console.WriteLine(debugTabs + "====================> using move[{0}]", bestMove);
		}

		requiredDepth = depth;
		return moves[bestMove];
	}
}
