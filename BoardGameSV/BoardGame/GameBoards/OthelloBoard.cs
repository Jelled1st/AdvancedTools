using System;
using System.Collections.Generic;

class OthelloBoard : SquareBoard {
	public delegate void CellChangeHandler(int row, int col, int value);
	public event CellChangeHandler OnCellChange=null; // Called when a cell changes

	public delegate void MarkHandler(int row, int col);
	public event MarkHandler OnMark; // Called to indicate cells where a move can be made

	byte[,] feasiblemove;
	List<int> feasiblemovelist;
	bool end=false;

	public OthelloBoard(int width=8, int height=8, bool alternative=false) : base(width,height) {
		feasiblemove = new byte[height, width];
		feasiblemovelist = new List<int> ();
		Reset (alternative);
	}

	public OthelloBoard(OthelloBoard orig) : base(orig._width,orig._height) {
		feasiblemove = new byte[_height, _width];
		feasiblemovelist = new List<int> ();
		for (int i = 0; i < _height; i++)
			for (int j = 0; j < _width; j++)
				board [i, j] = orig.board[i,j];	
		activeplayer = orig.activeplayer;
		movesmade = orig.movesmade;
	}

	public override GameBoard Clone() {
		return new OthelloBoard (this);
	}

	public override void Reset() {
		Reset (false);
	}

	// <alternative>: there are two common start configurations for Othello. Set to true for the non-standard one.
	public void Reset(bool alternative) {
		base.Reset ();
		board [_height / 2 - 1, _width / 2 - 1] = -1;
		board [_height / 2    , _width / 2 - 1] = 1;
		board [_height / 2 - 1, _width / 2    ] = (sbyte)((alternative)?-1:1);
		board [_height / 2    , _width / 2    ] = (sbyte)((alternative)?1:-1);
		for (int i = 0; i < _height; i++)
			for (int j = 0; j < _width; j++) {
				if (OnCellChange!=null)
					OnCellChange (i, j,board[i,j]);
			}
		end = false;
	}

	bool CheckFeasibleMove(int row, int col, int rowdir, int coldir, int player, bool makemove=false) {		
		// just for debug:
		//int origrow=row;
		//int origcol=col;
		//string origboard = ToString ();

		if (board[row,col]!=0) 
			return false;
		try {
			row+=rowdir;
			col+=coldir;
			if (row<0 || row>=_height || col<0 || col>=_width || board[row,col]!=-player)
				return false;
			while (board[row,col]==-player) {
				row+=rowdir;
				col+=coldir;
				if (row<0 || row>=_height || col<0 || col>=_width)
					return false;
			}
			if (board[row,col]==0)
				return false;
			if (makemove) {
				do {
					row-=rowdir;
					col-=coldir;
					board[row,col]=(sbyte)player;
					if (OnCellChange != null)
						OnCellChange (row, col, player);
				} while (board[row-rowdir,col-coldir]==-player);
			}
			return true;				
		} catch (IndexOutOfRangeException) {
			Console.WriteLine ("Whoops! row {0} col {1} rowdir {2} coldir {3} makemove {5} player {6} board:\n{4}",
					row,col,rowdir,coldir,ToString(),makemove,player);
			//Console.WriteLine ("Whoops! row {0} col {1} rowdir {2} coldir {3} makemove {5} player {6} board:\n{4}orig row: {7} orig col: {8} orig board:\n{9}",
			//	row,col,rowdir,coldir,ToString(),makemove,player,origrow,origcol,origboard);
			return false;
		}
	}

	bool CheckFeasibleMove(int row, int col, int player, bool makemove=false) {
		if (board [row, col] != 0)
			return false;
		bool valid = false;
		for (int rowdir = -1; rowdir <= 1; rowdir++)
			for (int coldir = -1; coldir <= 1; coldir++) {
				valid|=CheckFeasibleMove(row,col,rowdir,coldir,player,makemove);
			}
		return valid;
	}


	// Marks the feasible moves 
	// [row,col] is the startposition, and [rowdir,rowcol] the direction. coldir must be nonnegative.
	void MarkFeasibleMoves(int row, int col, int rowdir, int coldir, int player) {
		int lastemptyrow = -1;
		int lastemptycol = -1;

		bool checkstart = false;
		bool twotypes = false;
		int lastcell = -player;
		while (row < _height && col < _width && row >= 0) {
			if (board [row, col] == 0) { 
				if (twotypes) {
					if (lastcell == -player) {
						feasiblemove [row, col] = 1;	// for now, just mark it. (end of sequence)
					}
				}
				lastcell = 0;
				checkstart = false;
				twotypes = false;
			} else if (board [row, col] == -player) {
				if (lastcell == 0) {
					checkstart = true;
					lastemptyrow = row - rowdir;
					lastemptycol = col - coldir;
				}
				if (lastcell == player) {
					twotypes = true;
					/*
					if (checkstart) {
						feasiblemove [lastemptyrow, lastemptycol] = 1; // mark start of sequence
						checkstart=false;
					}
					*/
				}
				lastcell = -player;
			} else { // currentcell == player
				if (lastcell == -player) {
					twotypes = true;
					if (checkstart) {
						feasiblemove [lastemptyrow, lastemptycol] = 1; // mark start of sequence
						checkstart=false;
					}
				}
				lastcell = player;
			}
			row += rowdir;
			col += coldir;
		}
	}

	public override List<int> GetMoves() {
		//Console.WriteLine ("Computing all feasible moves...");
		List<int> output = new List<int>();
		feasiblemove = new byte[_height, _width];	// for now...
		for (int row=0;row<_height;row++) {
			MarkFeasibleMoves (row, 0, 0, 1, activeplayer);	// Row
			if (row>=2)
				MarkFeasibleMoves (row, 0, -1, 1, activeplayer);	// right-up diagonal
			if (row < _height - 2)
				MarkFeasibleMoves (row, 0, 1, 1, activeplayer);	// right-down diagonal
		}
		for (int col = 0; col < _width; col++) {
			MarkFeasibleMoves (0, col, 1, 0, activeplayer);	// column
			if (col < _width - 2) {
				MarkFeasibleMoves (0, col, 1, 1, activeplayer);	// right-down diagonal
				MarkFeasibleMoves (_height - 1, col, -1, 1, activeplayer); // right-up diagonal
			}
		}

		for (int row = 0; row < _height; row++)
			for (int col = 0; col < _width; col++)
				if (feasiblemove [row, col] != 0) {//(CheckFeasibleMove(row,col,activeplayer))
					output.Add (row * _width + col);
					if (OnMark!=null)
						OnMark (row, col);
				}
		return output;
	}

	public override string MoveToString(int move) {
		return (move / _width).ToString () + "," + (move % _width).ToString ();
	}

	public override int MakeMove(int move) {
		int row = move / _width;
		int col = move % _width;
		if (!CheckFeasibleMove (row, col, activeplayer, true)) {
			Console.WriteLine ("Invalid move: player {3} row {0} col {1} on board:\n{2}", row, col, ToString(),Symbol(activeplayer));
			throw new Exception ("Invalid move!");
		} else {
			board [row, col] = (sbyte)activeplayer;
			if (OnCellChange != null)
				OnCellChange (row, col, activeplayer);
			activeplayer = -activeplayer;
			List<int> moves = GetMoves ();
			if (moves.Count == 0) {
				activeplayer = -activeplayer;
				moves = GetMoves ();
				if (moves.Count == 0) {
					end=true;
				}
			}
		}
		movesmade++;
		return 0;
	}

	public int CountStones() {
		int total=0;
		for (int row = 0; row < _height; row++)
			for (int col = 0; col < _width; col++)
				total += board [row, col];
		return total;
	}

	public override int CheckWinner() {
		if (!end)
			return 0;
		int total = CountStones ();
		if (OnCellChange!=null) // only give feedback for viewed boards (=main board)
			Console.WriteLine ("Stone difference: {0} in favor of player {1}", Math.Abs (total), Symbol (Math.Sign (total)));
		return Math.Sign (total);
	}

	public override int MaxMovesLeft() {
		return _width * _height - movesmade-4;
	}
}
