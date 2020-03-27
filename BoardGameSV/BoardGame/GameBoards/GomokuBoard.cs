using System.Collections.Generic;
using System;

class GomokuBoard : SquareBoard {
	public delegate void CellChangeHandler(int row, int col, int value);
	public event CellChangeHandler OnCellChange=null;	// Called when a cell changes

	public readonly int _winlength;
	public readonly int _maxlength;

	bool winchecked; // True iff CheckWinner has been called after the last MakeMove.
	bool terminal; // True iff the current state is a terminal state.

	public GomokuBoard(int width=9, int height=9, int winlength=5, int maxlength=5) : base(width,height) {
		_winlength = winlength;
		_maxlength = maxlength;
		winchecked = false;
		terminal = false;
	}

	public GomokuBoard(GomokuBoard orig) : base(orig._width,orig._height) {
		_winlength = orig._winlength;
		_maxlength = orig._maxlength;
		for (int i = 0; i < _height; i++)
			for (int j = 0; j < _width; j++)
				board [i, j] = orig.board[i,j];	
		activeplayer = orig.activeplayer;
		movesmade = orig.movesmade;
		winchecked = orig.winchecked;
		terminal = orig.terminal;
	}

	public override GameBoard Clone() {
		return new GomokuBoard (this);
	}

	public override void Reset() {
		base.Reset ();
		if (OnCellChange!=null)
			for (int i = 0; i < _height; i++)
				for (int j = 0; j < _width; j++) 
					OnCellChange (i, j,0);
		winchecked = false;
		terminal = false;
	}

	public override List<int> GetMoves() {
		List<int> output = new List<int> ();
		if (!winchecked)
			CheckWinner ();	// This may set terminal to true
		if (terminal)			
			return output;	// Return empty move list in terminal state
		for (int row=0;row<_height;row++)
			for (int col=0;col<_width;col++) {
				if (board [row,col] == 0)
					output.Add (row*_width+col);
			}
		return output;
	}

	// Checks whether the board position [row,col] (typically the last move) is part of a winning 4-sequence
	// Btw this doesn't notify the views!
	public int CheckWinner(int row, int col) {
		winchecked = true;	// Now you can only mess it up if you pass the wrong move number to the CheckWinner method - so don't!!!		
		if (board [row, col] == 0)
			return 0;
		terminal = true;
		int result = 0;
		result = countaround (row, col, 1, 0);
		if (result >= _winlength && result<=_maxlength)
			return board [row, col];
		result = countaround (row, col, 0, 1);
		if (result >= _winlength && result<=_maxlength)
			return board [row, col];
		result = countaround (row, col, 1, 1);
		if (result >= _winlength && result<=_maxlength)
			return board [row, col];
		result = countaround (row, col, 1, -1);
		if (result >= _winlength && result<=_maxlength)
			return board [row, col];
		terminal = false;
		return 0;
	}

	public override int CheckWinner(int move) {
		int row = move/_width;
		int col = move % _width;
		return CheckWinner (row, col);
	}

	public override int CheckWinner() {
		winchecked = true;
		int[] playerwins = CheckWinSequences (_winlength, _maxlength);
		if (playerwins [0] > 0) {
			terminal = true;
			return -1;
		}
		if (playerwins [1] > 0) { 
			terminal = true;
			return 1;
		}
		return 0;
	}

	public override string MoveToString(int move) {
		return (move / _width).ToString () + "," + (move % _width).ToString ();
	}

	public override int MakeMove(int move) {
		int row = move / _width;
		int col = move % _width;
		if (board [row, col] != 0)
			return -1;

		// The move is valid; make it:
		winchecked = false;
		board [row, col] = (sbyte)activeplayer;
		// notify all views of this change:
		if (OnCellChange!=null)
			OnCellChange (row, col, activeplayer);
		activeplayer = -activeplayer;
		movesmade++;
		return 1;
	}
}
