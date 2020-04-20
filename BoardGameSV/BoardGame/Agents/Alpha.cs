using System.Collections.Generic;
using System;
using GXPEngine;	// Use this for Time

namespace AgentMain {

	class Alpha : Agent
	{
		static Random myrandom = new Random();

		private int _searchDepth = -1;

		//if true it will use score > currentBestScore
		//else if false it will use >= currentBestScore
		private bool _onlyBetterScore;

		public Alpha(string name, int pSearchDepth = -1, bool pOnlyBetterScore = true) : base(name)
		{
			_searchDepth = pSearchDepth;
			_onlyBetterScore = pOnlyBetterScore;
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

			List<int> moves = current.GetMoves();
			if (ID == -1)
			{
				bestValue = 200;
				for (int m = 0; m < moves.Count; ++m)
				{
					GameBoard clone = current.Clone();
					clone.MakeMove(moves[m]);
					//value is actually the winner of the game
					int score = getMove(clone, 0, -200, 200);

					if (_onlyBetterScore && score <= bestValue)
					{
						bestValue = score;
						bestMove = m;
					}
					else if (!_onlyBetterScore && score < bestValue)
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
					int score = getMove(clone, 0, -200, 200);

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

		private int getMove(GameBoard board, int depth, int alpha, int beta)
		{
			string tabs = "";
			for (int i = 0; i < depth; ++i)
			{
				tabs += "\t";
			}
			//Console.WriteLine(tabs + "Called with alpha {0} and beta {1}", alpha, beta);
			int winner = board.CheckWinner();
			if (depth == _searchDepth || winner != 0 || board.MaxMovesLeft() == 0)
			{
				return winner;
			}

			int[] moves = new int[board.GetMoves().Count];
			board.GetMoves().CopyTo(moves);
			int bestMove = 0;
			int bestValue = 0;

			//if-statement first, because otherwise it would have to execute the if-statement as many times as the loop loops
			if (board.GetActivePlayer() == -1)
			{
				bestValue = 200;
				GameBoard clone = board.Clone();
				for (int m = 0; m < moves.Length; ++m)
				{
					clone.MakeMove(moves[m]);
					//value is actually the winner of the game
					int score = getMove(clone, depth + 1, alpha, beta);
					//beta = Math.Min(beta, score);
					clone.UndoLastMove();

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

					if (beta > bestValue)
					{
						beta = score;
						//Console.WriteLine(tabs + "set beta. a{0} vs b{1}", alpha, beta);
					}
					if (beta <= alpha)
					{
						//Console.WriteLine(tabs + "beta was lower than alpha");
						break;
					}
				}
			}
			else
			{
				bestValue = -200;
				GameBoard clone = board.Clone();
				for (int m = 0; m < moves.Length; ++m)
				{
					clone.MakeMove(moves[m]);
					//value is actually the winner of the game
					int score = getMove(clone, depth + 1, alpha, beta);
					//alpha = Math.Max(alpha, score);
					clone.UndoLastMove();

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

					if (alpha < bestValue)
					{
						alpha = score;
						//Console.WriteLine(tabs + "set alpha. a{0} vs b{1}", alpha, beta);
					}
					if (beta <= alpha)
					{
						//Console.WriteLine(tabs + "beta was lower than alpha");
						break;
					}
				}
			}
			return bestValue;
		}
	}

}