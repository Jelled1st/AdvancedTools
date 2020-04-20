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

	private bool _debugInfo = false;

	public MiniCarlo(string name, int pSearchDepth, int pSamples = 25, bool pGreedyRandomPlay = true, bool pOnlyBetterScore = true) : base(name)
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
		float bestScore = 0;

		List<int> moves = current.GetMoves();
		if (ID == -1)
		{
			bestScore = 200;
			for (int m = 0; m < moves.Count; ++m)
			{
				GameBoard clone = current.Clone();
				clone.MakeMove(moves[m]);
				//value is actually the winner of the game
				float score = getScore(clone, 0);

				if (_onlyBetterScore && score <= bestScore)
				{
					bestScore = score;
					bestMove = m;
				}
				else if (!_onlyBetterScore && score < bestScore)
				{
					bestScore = score;
					bestMove = m;
				}
			}
		}
		else
		{
			bestScore = -200;
			for (int m = 0; m < moves.Count; ++m)
			{
				GameBoard clone = current.Clone();
				clone.MakeMove(moves[m]);
				//value is actually the winner of the game
				float score = getScore(clone, 0);

				if (_onlyBetterScore && score >= bestScore)
				{
					bestScore = score;
					bestMove = m;
				}
				else if (!_onlyBetterScore && score > bestScore)
				{
					bestScore = score;
					bestMove = m;
				}
			}
		}

		//Console.WriteLine(name+": Hm let's try this move...");
		return moves[bestMove];
	}

	//tooDeepNoWinner returns whther the search ended because of search depth and if there 
	private float getScore(GameBoard board, int depth)
	{
		int winner = board.CheckWinner(); //times two, because if the minimax finds a winner it counts extra
		if (depth == _searchDepth || winner != 0 || board.MaxMovesLeft() == 0)
		{
			float score = winner*2;
			if (winner != 0)
			{
				if (_debugInfo) Console.WriteLine("Score found by Minimax, {0}", score);
			}
			if (depth == _searchDepth && score == 0)
			{
				// search depth has been reached and no winner :(
				// use monte carlo from here
				if (_debugInfo) Console.WriteLine("Reached search depth: switching to monte carlo...\nPlaying {0} games", _monteCarloSamples);
				int wins = 0;
				int losses = 0;
				int player = board.GetActivePlayer();
				for(int i = 0; i < _monteCarloSamples; ++i)
				{
					int outcome;
					if (_greedyRandomPlay)
					{
						outcome = greedyRandomPlay(board);
					}
					else outcome = randomPlay(board);
					if (outcome == player) ++wins;
					else if (outcome == -player) ++losses;
				}
				//if (wins > losses) winner = player;
				//else if (losses > wins) winner = -player;
				//else winner = 0;
				score = (wins - losses) / (float)_monteCarloSamples * player; // *player will either keep it the same (*1) or will make it negative (*-1)
				if (_debugInfo) Console.WriteLine("Games have been played, wins {0}, losses {1}, SCORE {2}", wins, losses, score);
			}
			if (_debugInfo) Console.WriteLine("Returning score {0}\n", score);
			return score;
		}

		List<int> moves = board.GetMoves();
		int bestMove = 0;
		float bestScore = 0;

		//if-statement first, because otherwise it would have to execute the if-statement as many times as the loop loops
		if (board.GetActivePlayer() == -1)
		{
			bestScore = 200;
			for (int m = 0; m < moves.Count; ++m)
			{
				GameBoard clone = board.Clone();
				clone.MakeMove(moves[m]);
				//value is actually the winner of the game
				float score = getScore(clone, depth + 1);

				if (_onlyBetterScore && score <= bestScore)
				{
					bestScore = score;
					bestMove = m;
				}
				else if (!_onlyBetterScore && score < bestScore)
				{
					bestScore = score;
					bestMove = m;
				}
			}
		}
		else
		{
			bestScore = -200;
			for (int m = 0; m < moves.Count; ++m)
			{
				GameBoard clone = board.Clone();
				clone.MakeMove(moves[m]);
				//value is actually the winner of the game
				float score = getScore(clone, depth + 1);

				if (_onlyBetterScore && score >= bestScore)
				{
					bestScore = score;
					bestMove = m;
				}
				else if (!_onlyBetterScore && score > bestScore)
				{
					bestScore = score;
					bestMove = m;
				}
			}
		}
		return bestScore;
	}

	private int randomPlay(GameBoard pBoard)
	{
		GameBoard board = pBoard.Clone();
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
		if (_debugInfo) Console.WriteLine("========================> random game finished after {0} loops, winner {1}", loops, winner);
		return winner;
	}

	private int greedyRandomPlay(GameBoard pBoard)
	{
		GameBoard board = pBoard.Clone();
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
				int activePlayer = clone.GetActivePlayer();
				clone.MakeMove(moves[m]);
				int tempWinner = clone.CheckWinner();
				if (tempWinner == activePlayer) //check if the winner is the one who just made the move
				{
					winner = tempWinner;
					if (_debugInfo) Console.WriteLine("Found a winning move for {0}", winner);
					break;
				}
			}
			
			if (winner != 0) break;
			int random = myrandom.Next(0, moves.Count);
			board.MakeMove(moves[random]);
			++loops;
		}
		if(_debugInfo) Console.WriteLine("========================> random game finished after {0} loops, winner {1}", loops, winner);
		return winner;
	}
}
