using System;

// This class can be used to represent finite two player games that are played on a square board, 
// where cells can contain the values -1, 0 and 1 (player 1 stone, empty, player 2 stone resp.),
// It contains some general helper functions that are useful for Connect Four and many similar games.
abstract class SquareBoard : GameBoard {
	public delegate void WinHandler(int startrow, int startcol, int rowdir, int coldir, int length);
	public event WinHandler OnWin;			// Called when the game is over

	protected sbyte[,] board;
	public readonly int _width,_height; 
	protected int movesmade;

	public SquareBoard(int width, int height) {
		_width = width;
		_height = height;
		board = new sbyte[height,width];
		movesmade = 0;
	}

	public override string ToString() {
		string output="";
		for (int i=0;i<_height;i++) {
			for (int j=0;j<_width;j++) {
				output += " " + Symbol (board [i, j]);
			}
			output+="\n";
		}
		for (int j = 0; j < _width; j++) {
			output += " " + j.ToString ();
		}
		output += "\n";
		return output;
	}

	// Resets the board.
	// NB: Views need to be notified in subclasses!
	public override void Reset() {
		for (int i = 0; i < _height; i++)
			for (int j = 0; j < _width; j++) {
				board [i, j] = 0;		
			}
		activeplayer = 1;	
		movesmade = 0;
	}

	// This method ensures that indexing is supported; use e.g. SquareBoard[i,j] to get the cell value at row i, column j.
	// Use this e.g. for smart custom evaluation functions.
	public sbyte this[int row,int col] {
		get {
			return board [row, col];
		}
	}

	// Use this method to get an array representation of the board.
	// Use this e.g. for smart custom evaluation functions.
	public sbyte[,] ToArray() {
		return (sbyte[,])(board.Clone ());
	}

	// In games where every move fills one empty board cell, and the game ends when all cells are filled, this is the correct formula.
	// (Override this method for other games)
	public override int MaxMovesLeft () {
		return _width * _height - movesmade;
	}

	// Counts the length of the maximal straight sequence of cells around cell <board[row,col]> that all have the same value as <board[row,col]>, 
	// in the direction of <dirx> & <diry> (indicating e.g. a row, column, diagonal).
	// 
	// Use this for fast win checks in Connect-Four boards and similar boards as follows:
	// <row,col> is typically the last move; then only the row, column, down diagonal and up diagonal need to be checked, and the 
	// resulting length can be compared to <_winlength> to see if the last move was a winning move.
	public int countaround(int row,int col,int dirx, int diry) {
		if (dirx < 0) {
			dirx *= -1;
			diry *= -1;
		}
		int origval = board [row,col];
		int total = 1;
		int currentx = col+dirx;
		int currenty = row+diry;
		while (currentx < _width && currenty < _height && currenty >= 0 && board [currenty, currentx] == origval) {
			total += 1;
			currentx += dirx;
			currenty += diry;
		}
		currentx = col-dirx;
		currenty = row-diry;
		while (currentx >=0 && currenty < _height && currenty >= 0 && board [currenty, currentx] == origval) {
			total += 1;
			currentx -= dirx;
			currenty -= diry;
		}
		//Console.WriteLine ("Counting {0},{1} in direction {2},{3}: {4}", row, col, dirx,diry,total);
		return total;
	}

	// Returns 1 (resp. -1) if the given row/column/diagonal contains a maximal sequence of 1's (resp. -1's) with length
	// between [minlength] and [maxlength] - a "winning sequence".
	//
	// Use [row] and [col] to indicate the start position, and [dirx] and [diry] to indicate the direction.
	// (For example, for checking row 3 use [row]=3, [col]=0, [dirx]=1, [diry]=0.)
	//
	// NB if the row/column/diagonal contains "winning sequences" for both players, only the first is considered! For all of the 
	// games implemented currently this is no problem though.
	protected int countfull(int row, int col, int dirx, int diry, int minlength, int maxlength) { 
		//Console.Write ("Checking row {0} col {1} rowdir {2} coldir {3} minlength {4}", row, col, diry, dirx, minlength);
		int lastval = 0;
		int sequencelength = 0;
		while (row >= 0 && row < _height && col >= 0 && col < _width) {
			if (board [row, col] == lastval) {
				sequencelength++;
			} else { // End of maximal sequence - now test whether it is a winning sequence:
				if (lastval != 0 && sequencelength >= minlength && sequencelength <= maxlength) {
					//Console.WriteLine (" - win for {0}", lastval);
					if (OnWin != null)
						OnWin (row - diry, col - dirx, -diry, -dirx, sequencelength);
					return lastval;
				}
				sequencelength = 1;
				lastval = board [row, col];
			}
			row += diry;
			col += dirx;
		}
		if (lastval != 0 && sequencelength >= minlength && sequencelength <= maxlength) {
			//Console.WriteLine (" - win for {0}", lastval);
			if (OnWin != null)
				OnWin (row - diry, col - dirx, -diry, -dirx, sequencelength);
			return lastval;
		}
		//Console.WriteLine (" - got nothing");
		return 0;
	}

	// A useful utility function for connect-four boards and similar boards:
	// Counts the number of row/column/diagonal sequences between length [minlength] and [maxlength] (inclusive) for both players.
	// (Connect-four: use [minlength]=4, [maxlength]=[_width], etc.)
	protected int[] CheckWinSequences(int minlength, int maxlength) {
		int[] playerwins = new int[2];
		int result;
		int row = 0;
		int col = 0;

		// Rows:
		for (row = 0; row < _height; row++) {
			result = countfull (row, 0, 1, 0, minlength, maxlength);
			if (result!=0)
				playerwins [(result + 1) / 2]++;
		}
		// Columns:
		for (col = 0; col < _width; col++) {
			result = countfull (0,col, 0, 1, minlength, maxlength);
			if (result!=0)
				playerwins [(result + 1) / 2]++;
		}
		// Down diagonals:
		for (int rowMinusCol = -_width + minlength; rowMinusCol <= _height - minlength; rowMinusCol++) {
			row = 0;
			col = 0;
			if (rowMinusCol < 0) {
				col = -rowMinusCol;
			} else {
				row = rowMinusCol;
			}
			result = countfull (row, col, 1, 1, minlength, maxlength);
			if (result!=0)
				playerwins [(result + 1) / 2]++;
		}
		// Up diagonals:
		for (int rowPlusCol = minlength-1; rowPlusCol < _width + _height - minlength; rowPlusCol++) {
			row = _height-1;
			col = 0;
			if (rowPlusCol < _height) {
				row = rowPlusCol;
			} else {
				col = rowPlusCol-row;
			}
			result = countfull (row, col, 1, -1, minlength, maxlength);
			if (result!=0)
				playerwins [(result + 1) / 2]++;
		}
		//Console.WriteLine ("Number of sequences with {0}<=length<={1}: p1: {2} p2: {3}", minlength, maxlength, playerwins [0], playerwins [1]);
		return playerwins;
	}

	public override bool isOpeningMove()
	{
		return movesmade == 0;
	}

	public override int GetBestOpeningMove()
	{
		return _width * _height / 2;
	}
}
