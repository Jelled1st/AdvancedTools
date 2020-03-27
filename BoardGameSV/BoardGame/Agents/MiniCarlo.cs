using System;
using System.Collections.Generic;

//MiniCarlo is a combinaiton between MonteCarlo and MiniMax
class MiniCarlo : Agent
{
	static Random myrandom = new Random();

	private int _searchDepth = -1;

	//if true it will use score > currentBestScore
	//else if false it will use >= currentBestScore
	private bool _onlyBetterScore;
	private bool _greedyRandomPlay;

	private int _monteCarloSamples;

	public MiniCarlo(string name, int pSearchDepth = -1, int pSamples = 25, bool pGreedyRandomPlay = true, bool pOnlyBetterScore = true) : base(name)
	{
		_searchDepth = pSearchDepth;
		_onlyBetterScore = pOnlyBetterScore;
		_greedyRandomPlay = pGreedyRandomPlay;
		_monteCarloSamples = pSamples;

	}

	public override int ChooseMove(GameBoard current, int timeLeftMS)
	{
		int ID = current.GetActivePlayer();
		Console.WriteLine(name + ": I'm playing as player {0}", ID);

		// TODO: Implement a recursive MiniMax algorithm here, using a Monte Carlo evaluation function, instead of the dumb algorithm below.

		//if(current.isOpeningMove())
		//{
		//	Console.WriteLine("do opening move");
		//	return current.GetBestOpeningMove();
		//}

		int bestMove = 0;
		int bestValue = 0;
		bool searchDepthReached = false;

		List<int> moves = current.GetMoves();
		if (ID == -1)
		{
			bestValue = 200;
			for (int m = 0; m < moves.Count; ++m)
			{
				GameBoard clone = current.Clone();
				clone.MakeMove(moves[m]);
				//value is actually the winner of the game
				bool depthReached;
				int score = getMove(clone, 0, out depthReached);

				if (_onlyBetterScore && score <= bestValue)
				{
					bestValue = score;
					bestMove = m;
					searchDepthReached = depthReached;
				}
				else if (!_onlyBetterScore && score < bestValue)
				{
					bestValue = score;
					bestMove = m;
					searchDepthReached = depthReached;
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
				bool depthReached;
				int score = getMove(clone, 0, out depthReached);

				if (_onlyBetterScore && score >= bestValue)
				{
					bestValue = score;
					bestMove = m;
					searchDepthReached = depthReached;
				}
				else if (!_onlyBetterScore && score > bestValue)
				{
					bestValue = score;
					bestMove = m;
					searchDepthReached = depthReached;
				}
			}
		}

		//Console.WriteLine(name+": Hm let's try this move...");
		return moves[bestMove];
	}

	//tooDeepNoWinner returns whther the search ended because of search depth and if there 
	private int getMove(GameBoard board, int depth, out bool searchDepthReached)
	{
		int winner = board.CheckWinner();
		if (depth == _searchDepth || winner != 0 || board.MaxMovesLeft() == 0)
		{
			if (depth == _searchDepth)
			{
				// search depth has been reached and no winner :(
				// use monte carlo from here
				searchDepthReached = true;
			}
			else searchDepthReached = false;
			return winner;
		}

		List<int> moves = board.GetMoves();
		int bestMove = 0;
		int bestValue = 0;
		searchDepthReached = false;

		//if-statement first, because otherwise it would have to execute the if-statement as many times as the loop loops
		if (board.GetActivePlayer() == -1)
		{
			bestValue = 200;
			for (int m = 0; m < moves.Count; ++m)
			{
				GameBoard clone = board.Clone();
				clone.MakeMove(moves[m]);
				//value is actually the winner of the game
				bool depthReached = false;
				int score = getMove(clone, depth + 1, out depthReached);

				if (_onlyBetterScore && score <= bestValue)
				{
					bestValue = score;
					bestMove = m;
					searchDepthReached = depthReached;
				}
				else if (!_onlyBetterScore && score < bestValue)
				{
					bestValue = score;
					bestMove = m;
					searchDepthReached = depthReached;
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
				bool depthReached = false;
				int score = getMove(clone, depth + 1, out depthReached);

				if (_onlyBetterScore && score >= bestValue)
				{
					bestValue = score;
					bestMove = m;
					searchDepthReached = depthReached;
				}
				else if (!_onlyBetterScore && score > bestValue)
				{
					bestValue = score;
					bestMove = m;
					searchDepthReached = depthReached;
				}
			}
		}
		return bestValue;
	}

	private int MonteCarloMove(GameBoard board, int playerId)
	{
		List<int> moves = board.GetMoves();

		List<int> wins = new List<int>();
		List<int> losses = new List<int>();
		List<float> scores = new List<float>();
		int bestIndex = 0;
		Console.WriteLine("\nconsidering all movements::\n------------------------------");
		Console.WriteLine("Current active player: " + board.GetActivePlayer());
		for (int i = 0; i < moves.Count; ++i)
		{
			wins.Add(0);
			losses.Add(0);
			for (int s = 0; s < _monteCarloSamples; ++s)
			{
				GameBoard afterMove = board.Clone();
				afterMove.MakeMove(moves[i]);
				//Console.WriteLine("playing game number " + s + " amount of wins: " + wins[i]);
				int randomWinner;
				if (_greedyRandomPlay) randomWinner = greedyRandomPlay(afterMove);
				else randomWinner = randomPlay(afterMove);
				if (randomWinner == playerId)
				{
					++wins[i];
				}
				else if (randomWinner == -playerId)
				{
					++losses[i];
				}
			}

			float score = (wins[i] - losses[i]) / (float)_monteCarloSamples;
			scores.Add(score);
			if (scores[i] > scores[bestIndex]) bestIndex = i;
		}

		//Console.WriteLine(name+": Hm let's try this move...");
		Console.WriteLine(name + ": played " + moves.Count + "*" + _monteCarloSamples + " games");
		string output = "";
		for (int i = 0; i < wins.Count; ++i)
		{
			output += "\tmove[" + i + "]: " + moves[i] + " resulted in " + wins[i] + " wins and " + losses[i] + " losses, score of " + scores[i] + "\n";
		}
		output += "\t\tbest index is " + bestIndex + " with move: " + moves[bestIndex];
		Console.WriteLine(output);

		return moves[bestIndex];
	}

	private int getMoveOld(GameBoard board, int depth, out int requiredDepth)
	{
		string debugTabs = Helper.multiply("\t", depth);
		List<int> moves = board.GetMoves();
		Dictionary<int, int> moveValues = new Dictionary<int, int>();
		Dictionary<int, int> moveDepths = new Dictionary<int, int>();

		Console.WriteLine(debugTabs + "found {0} possible moves", moves.Count);

		int bestMove = 0;
		for (int i = 0; i < moves.Count; ++i)
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
			if (board.GetActivePlayer() == 1)
			{
				if (moveValues[i] > moveValues[bestMove])
				{
					Console.WriteLine(debugTabs + "this move was the best yet");
					bestMove = i;
				}
			}
			else if (board.GetActivePlayer() == -1)
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

	private int randomPlay(GameBoard board)
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
			for (int m = 0; m < moves.Count; ++m)
			{
				GameBoard clone = board.Clone();
				clone.MakeMove(moves[m]);
				if (clone.CheckWinner() == -clone.GetActivePlayer()) //check if the winner is the one who just made the move
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
