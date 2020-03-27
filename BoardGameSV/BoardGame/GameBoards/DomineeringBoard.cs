using System.Collections.Generic;
using System;

class DomineeringBoard : SquareBoard {
	public delegate void CellChangeHandler(int row, int col, int value);
	public event CellChangeHandler OnCellChange=null; // Called when a cell changes\

	public DomineeringBoard(int width=8, int height=8) : base(width,height) {
	}

	public DomineeringBoard(DomineeringBoard orig) : base(orig._width,orig._height) {
		for (int i = 0; i < _height; i++)
			for (int j = 0; j < _width; j++)
				board [i, j] = orig.board[i,j];	
		activeplayer = orig.activeplayer;
		movesmade = orig.movesmade;
	}

	public override GameBoard Clone() {
		return new DomineeringBoard (this);
	}

	public override void Reset() {
		base.Reset ();
		if (OnCellChange!=null)
			for (int i = 0; i < _height; i++)
				for (int j = 0; j < _width; j++) 
					OnCellChange (i, j,0);
	}

	public override List<int> GetMoves() {
		// player 1: positive moves. Player -1: negative moves.
		int rowoffset=0;
		int coloffset=1;
		if (activeplayer == 1) {
			rowoffset = 1;
			coloffset = 0;
		} 	
		List<int> output = new List<int> ();
		for (int row=0;row<_height-rowoffset;row++)
			for (int col=0;col<_width-coloffset;col++) {
				if (board [row,col] == 0 && board[row+rowoffset,col+coloffset]==0)
					output.Add ((1+row*_width+col)*activeplayer);
			}
		return output;
	}


	public override int CheckWinner() {
		if (GetMoves().Count == 0)
			return -activeplayer;
		return 0;
	}

	public override string MoveToString(int move) {
		int row = (Math.Abs (move) - 1) / _width;
		int col = (Math.Abs (move) - 1) % _width;
		if (move < 0)
			return row.ToString () + "," + col.ToString () + " + " + row.ToString () + "," + (col + 1).ToString ();
		else 
			return row.ToString () + "," + col.ToString () + " + " + (row+1).ToString () + "," + col.ToString ();
	}

	public override int MakeMove(int move) {
		int row = (Math.Abs(move)-1) / _width;
		int col = (Math.Abs(move)-1) % _width;
		int row2=row;
		int col2=col+1;
		if (activeplayer == 1) {
			row2 = row + 1;
			col2 = col;
		} 	
		if (board [row, col] != 0 || board[row2,col2]!=0)
			return -1;
		board [row, col] = (sbyte)activeplayer;
		board [row2, col2] = (sbyte)activeplayer;

		// notify all views of this change:
		if (OnCellChange != null) {
			OnCellChange (row, col, activeplayer);
			OnCellChange (row2, col2, activeplayer);
		}
		activeplayer = -activeplayer;
		movesmade++;
		return 1;
	}

	public override int MaxMovesLeft ()
	{
		return _width * _height / 2 - movesmade;
	}
}
