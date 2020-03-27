using System.Collections.Generic;
using System;

class PentagoBoard : SquareBoard {
	public delegate void CellChangeHandler(int row, int col, int value);
	public event CellChangeHandler OnCellChange=null;	// When a cell changes

	bool turn;	// True if the next move is a "turn" move
	bool terminal;	// True iff it is a terminal state. Including the states that are a draw because both players win simultaneously (can happen after a turn move).
	bool winchecked; // True iff CheckWinner has been called after the last MakeMove.

	public PentagoBoard() : base(6,6) {
		turn = false;
		terminal = false;
		winchecked = false;
	}

	public PentagoBoard(PentagoBoard orig) : base(6,6) {
		for (int i = 0; i < _height; i++)
			for (int j = 0; j < _width; j++)
				board [i, j] = orig.board[i,j];	
		activeplayer = orig.activeplayer;
		turn = orig.turn;
		terminal = orig.terminal;
		winchecked = orig.winchecked;
		movesmade = orig.movesmade;
	}

	public override GameBoard Clone() {
		return new PentagoBoard (this);
	}

	public override void Reset() {
		base.Reset ();
		if (OnCellChange!=null)
			for (int i = 0; i < _height; i++)
				for (int j = 0; j < _width; j++) 
					OnCellChange (i, j,0);
		turn = false;
		terminal = false;
		winchecked = false;
	}

	public override List<int> GetMoves() {
		List<int> output = new List<int> ();
		if (!winchecked)
			CheckWinner ();	// This may set terminal to true
		if (terminal)
			return output;	// Return empty move list in terminal state
		if (turn) {
			for (int i = 36; i < 44; i++)	// represents the eight possible turns
				output.Add (i);
		} else {
			for (int row = 0; row < _height; row++)
				for (int col = 0; col < _width; col++) {
					if (board [row, col] == 0)
						output.Add (row * _width + col);
				}
		}
		return output;
	}

	// Checks whether the board position [row,col] (typically the last move) is part of a winning 5-sequence
	// Btw this doesn't notify the views!
	public int CheckWinner(int row, int col) {
		winchecked = true;
		if (board [row, col] == 0)
			return 0;
		terminal = true;
		if (countaround (row, col, 1, 0) >= 5)
			return board [row, col];
		if (countaround (row, col, 0, 1) >= 5)
			return board [row, col];
		if (countaround (row, col, 1, 1) >= 5)
			return board [row, col];
		if (countaround (row, col, 1, -1) >= 5)
			return board [row, col];
		terminal = false;
		return 0;
	}

	void processcount(int row,int col,int dirx, int diry, ref int sum, ref int abssum) {
		if (board [row, col] == 0)
			return;
		if (countaround(row,col,dirx,diry) >= 5) {
			//Console.WriteLine ("Hit! row {0} col {1} dirx {2} diry {3}", row, col, dirx, diry);
			sum += board[row,col];
			abssum += 1;
		}
	}


	public override int CheckWinner(int move) {
		winchecked = true;
		if (move < 36) { // standard win check
			return CheckWinner (move/6, move%6);
		} else {
			move -= 36;
			int sum = 0;
			int abssum = 0;
			// rows:
			if (move % 2 == 0) {	// even: top
				processcount(0,2,1,0,ref sum, ref abssum);	 // row 0
				processcount(1,2,1,0,ref sum, ref abssum);	 // row 1
				processcount(2,2,1,0,ref sum, ref abssum);	 // row 2
			} else { // odd: bottom
				processcount(3,2,1,0,ref sum, ref abssum);	 // row 3
				processcount(4,2,1,0,ref sum, ref abssum);	 // row 4
				processcount(5,2,1,0,ref sum, ref abssum);	 // row 5
			}
			// columns:
			if ((move >> 1) % 2 == 0) {	// 2nd bit==0: left
				processcount(2,0,0,1,ref sum, ref abssum);	 // col 0
				processcount(2,1,0,1,ref sum, ref abssum);	 // col 1
				processcount(2,2,0,1,ref sum, ref abssum);	 // col 2
			} else {	// right
				processcount(2,3,0,1,ref sum, ref abssum);	 // col 3
				processcount(2,4,0,1,ref sum, ref abssum);	 // col 4
				processcount(2,5,0,1,ref sum, ref abssum);	 // col 5
			}
			// diagonals:
			if ((move & 3) == 0 || (move & 3) == 3) { // TL - BR diagonal
				processcount(2,1,1,1,ref sum, ref abssum); 	
				processcount(2,2,1,1,ref sum, ref abssum); 	
				processcount(1,2,1,1,ref sum, ref abssum); 	
			} else { // BL - TR diagonal
				processcount(3,1,1,-1,ref sum, ref abssum); 	
				processcount(3,2,1,-1,ref sum, ref abssum); 	
				processcount(4,2,1,-1,ref sum, ref abssum); 	
			}
			if (abssum > 0) {
				//Console.WriteLine ("We have a winner! (Or two) Sum: {0} Abssum {1} Move {2}", sum, abssum,MoveToString(move+36));
				//Console.WriteLine (ToString ());
				//Console.ReadKey ();
				terminal = true;
			}
			if (Math.Abs (sum) == abssum)
				return Math.Sign (sum);
			return 0;
		}
	}

	public override int CheckWinner() {
		winchecked = true;
		int[] playerwins = CheckWinSequences (5,6);
		if (playerwins [0] > 0 && playerwins [1] == 0) {
			terminal = true;
			return -1;
		}
		if (playerwins [1] > 0 && playerwins [0] == 0) {
			terminal = true;
			return 1;
		}
		if (playerwins [0] > 0 && playerwins [1] > 0)
			terminal = true;
		return 0;
	}

	public override string MoveToString(int move) {
		if (move < 36)
			return (move / _width).ToString () + "," + (move % _width).ToString ();
		else
			switch (move-36) {
			case 0:
				return "Turn TL  CW"; // "Turn top-left clockwise";
			case 1:
				return "Turn BL  CW"; // "Turn bottom-left clockwise";
			case 2:
				return "Turn TR  CW"; // "Turn top-right clockwise";
			case 3:
				return "Turn BR  CW"; // "Turn bottom-right clockwise";
			case 4:
				return "Turn TL ACW"; // "Turn top-left anticlockwise";
			case 5:
				return "Turn BL ACW"; // "Turn bottom-left anticlockwise";
			case 6:
				return "Turn TR ACW"; // "Turn top-right anticlockwise";
			default: // = 7
				return "Turn BR ACW"; //  "Turn bottom-right anticlockwise";
			}			
	}

	public override int MakeMove(int move) {
		if (move < 36) {
			if (turn)
				throw new Exception ("Next move should be turn");
			int row = move / _width;
			int col = move % _width;
			if (board [row, col] != 0)
				return -1;
			winchecked = false;
			board [row, col] = (sbyte)activeplayer;
			// notify all views of this change:
			if (OnCellChange != null)
				OnCellChange (row, col, activeplayer);
			//activeplayer = -activeplayer; 
			turn = true;
			movesmade++;
			return 1;
		} else {
			if (!turn)
				throw new Exception ("Next move should not be turn");
			winchecked = false;
			move -= 36;
			int centerrow = 1 + 3 * (move % 2);
			move >>= 1;
			int centercol = 1 + 3 * (move % 2);
			move >>= 1;
			if (move > 0) { // turn counterclockwise
				sbyte tmpval = board[centerrow - 1, centercol - 1];
				board [centerrow - 1, centercol - 1] = board [centerrow - 1, centercol + 1];
				board [centerrow - 1, centercol + 1] = board [centerrow + 1, centercol + 1];
				board [centerrow + 1, centercol + 1] = board [centerrow + 1, centercol - 1];
				board [centerrow + 1, centercol - 1] = tmpval;

				tmpval = board[centerrow - 1, centercol];
				board [centerrow - 1, centercol] = board [centerrow, centercol + 1];
				board [centerrow, centercol + 1] = board [centerrow + 1, centercol];
				board [centerrow + 1, centercol] = board [centerrow, centercol - 1];
				board [centerrow, centercol - 1] = tmpval;
			} else {
				// turn clockwise
				sbyte tmpval = board[centerrow + 1, centercol - 1];
				board [centerrow + 1, centercol - 1] = board [centerrow + 1, centercol + 1];
				board [centerrow + 1, centercol + 1] = board [centerrow - 1, centercol + 1];
				board [centerrow - 1, centercol + 1] = board [centerrow - 1, centercol - 1];
				board [centerrow - 1, centercol - 1] = tmpval;

				tmpval = board[centerrow + 1, centercol];
				board [centerrow + 1, centercol] = board [centerrow, centercol + 1];
				board [centerrow, centercol + 1] = board [centerrow - 1, centercol];
				board [centerrow - 1, centercol] = board [centerrow, centercol - 1];
				board [centerrow, centercol - 1] = tmpval;
			}
			if (OnCellChange != null) {
				for (int row=centerrow-1;row<=centerrow+1;row++)
					for (int col=centercol-1;col<=centercol+1;col++)
						OnCellChange (row, col, board[row,col]);
			}
			activeplayer = -activeplayer; 
			turn = false;
			return 1;
		}
	}

	public bool GetTurn() {
		return turn;
	}

	public override int MaxMovesLeft() {
		int emptycells = _width * _height - movesmade;
		return turn ? 2 * emptycells : 2 * emptycells + 1;
	}

}
