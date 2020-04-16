using System.Collections.Generic;
using System;

class ConnectFourBoard : SquareBoard {
	public delegate void CellChangeHandler(int row, int col, int value);
	public event CellChangeHandler OnCellChange = null; // Called when a cell changes

	bool winchecked; // True iff CheckWinner has been called after the last MakeMove.
	bool terminal; // True iff the current state is a terminal state.

	public ConnectFourBoard(int width = 7, int height = 6) : base(width, height) {
		winchecked = false;
		terminal = false;
	}

	public ConnectFourBoard(ConnectFourBoard orig) : base(orig._width, orig._height) {
		for (int i = 0; i < _height; i++)
			for (int j = 0; j < _width; j++)
				board[i, j] = orig.board[i, j];
		activeplayer = orig.activeplayer;
		movesmade = orig.movesmade;
		winchecked = orig.winchecked;
		terminal = orig.terminal;
	}

	public override GameBoard Clone() {
		return new ConnectFourBoard(this);
	}

	public override void Reset() {
		base.Reset();
		if (OnCellChange != null)
			for (int i = 0; i < _height; i++)
				for (int j = 0; j < _width; j++)
					OnCellChange(i, j, 0);
		winchecked = false;
		terminal = false;
	}

	public override List<int> GetMoves() {
		List<int> output = new List<int>();
		if (!winchecked)
			CheckWinner();  // This may set terminal to true
		if (terminal)
			return output;  // Return empty move list in terminal state
		for (int i = 0; i < _width; i++) {
			if (board[0, i] == 0)
				output.Add(i);
		}
		return output;
	}

	// Checks whether the board position [row,col] (typically the last move) is part of a winning 4-sequence
	// Btw this doesn't notify the views!
	public int CheckWinner(int row, int col) {
		winchecked = true;  // Now you can only mess it up if you pass the wrong move number to the CheckWinner method - so don't!!!		
		if (board[row, col] == 0)
			return 0;
		terminal = true;
		if (countaround(row, col, 1, 0) >= 4)
			return board[row, col];
		if (countaround(row, col, 0, 1) >= 4)
			return board[row, col];
		if (countaround(row, col, 1, 1) >= 4)
			return board[row, col];
		if (countaround(row, col, 1, -1) >= 4)
			return board[row, col];
		terminal = false;
		return 0;
	}

	public override int CheckWinner(int move) {
		int row = 0;
		while (board[row, move] == 0 && row < _height - 1)
			row++;
		return CheckWinner(row, move);
	}

	public override int CheckWinner() {
		winchecked = true;
		int[] playerwins = CheckWinSequences(4, _width);
		if (playerwins[0] > 0) {
			terminal = true;
			return -1;
		}
		if (playerwins[1] > 0) {
			terminal = true;
			return 1;
		}
		return 0;
	}

	public override int MakeMove(int col) {
		if (board[0, col] != 0)
			return -1;

		// The move is valid; make it:
		winchecked = false;
		int row = 0;
		while (row < _height - 1 && board[row + 1, col] == 0)
			row++;
		board[row, col] = (sbyte)activeplayer;
		// notify all views of this change:
		if (OnCellChange != null)
			OnCellChange(row, col, activeplayer);
		activeplayer = -activeplayer;
		movesmade++;
		moves.Add(col);
		return row;
	}

	public override void UndoLastMove()
	{
		if (moves.Count == 0) return;
		int move = moves[moves.Count - 1];
		moves.RemoveAt(moves.Count - 1);
		int row = 0;
		while (row < _height - 1 && board[row + 1, move] == 0)
			row++;
		board[row, move] = 0;
		activeplayer = -activeplayer;
	}

	public override int GetBestOpeningMove()
	{
		return 4;
	}
}
