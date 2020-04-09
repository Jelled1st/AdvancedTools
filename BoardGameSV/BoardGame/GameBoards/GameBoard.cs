using System.Collections.Generic;
using System;


/// <summary>
/// An abstract class that can represent all kinds of two player games.
/// </summary>
abstract class GameBoard {
	protected int activeplayer=1;

	public char Symbol(int playernum) {
		switch (playernum) {
		case -1:
			return 'O';
		case 1:
			return 'X';
		default:
			return '.';
		}
	}


	/// <summary>
	/// Creates a clone of the current game board.
	/// </summary>
	abstract public GameBoard Clone(); 

	/// <summary>
	/// Resets the gameboard to an empty board (=start state).
	/// </summary>
	abstract public void Reset(); 

	/// <summary>
	/// Returns a list of all possible moves for the currently active player. The list is empty if and only if this is a terminal state.
	/// </summary>
	/// <returns>The moves.</returns>
	abstract public List<int> GetMoves(); 

	/// <summary>
	/// Returns 1 or -1 if that player has won (1=Max, -1=Min).
	/// Returns 0 if no one has won yet (so it's a nonterminal state) or if it's a draw.
	/// </summary>
	/// <returns>The winner (-1,0 or 1).</returns>
	abstract public int CheckWinner(); 

	/// <summary>
	/// This method is just added for efficiency: it does the same as CheckWinner(), but faster for some game types, 
	/// *if* you pass the last move that was made as parameter. If not, the output is undefined.
	/// </summary>
	/// <returns>The winner (-1,0 or 1).</returns>
	/// <param name="move">The last move that was made.</param>
	virtual public int CheckWinner(int move) {
		return CheckWinner ();
	}

	/// <summary>
	/// Makes the given move for the currently active player: changes the game board, and in most cases switches the active player.
	/// </summary>
	/// <returns>-1 if the move was illegal. Otherwise 1, or a special value for specific games.</returns>
	/// <param name="move">The move: should be one of the moves from the list returned by GetMoves.</param>
	abstract public int MakeMove(int move);

	/// <summary>
	/// Returns the currently active player (1=Max or -1=Min).
	/// </summary>
	/// <returns>The active player.</returns>
	virtual public int GetActivePlayer() {
		return activeplayer;
	}

	/// <summary>
	/// Sets the currently active player. Use this to select the starting player on an empty game board, or possibly
	/// to create unusual game plays...
	/// </summary>
	/// <param name="player">Player.</param>
	virtual public void SetActivePlayer(int player) {
		activeplayer=player;
	}

	/// <summary>
	/// Returns a string representation of the given move number (mostly for debugging)
	/// </summary>
	/// <returns>The string representation.</returns>
	/// <param name="move">The move number (e.g. one of the values from the list returned by GetMoves).</param>
	virtual public string MoveToString(int move) {
		return move.ToString ();
	}

	abstract public bool isOpeningMove();

	abstract public int GetBestOpeningMove();

	/// <summary>
	/// Returns an upper bound on the maximum number of moves left on the board
	/// (All currently implemented games are finite)
	/// </summary>
	/// <returns>The maximum number of moves left on the board.</returns>
	abstract public int MaxMovesLeft ();

	abstract public int GetMaxMoves();
}
