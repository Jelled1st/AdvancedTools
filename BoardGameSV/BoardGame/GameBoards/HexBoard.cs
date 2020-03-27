using System;
using System.Collections.Generic;

class HexBoard : SquareBoard {	
	public delegate void CellChangeHandler(int row, int col, int value);
	public event CellChangeHandler OnCellChange=null;	// Called when a cell changes

	public delegate void HexWinHandler(int startrow, int startcol, int winner);
	public event HexWinHandler OnHexWin;			// Called when the game is over

	protected struct coord {
		public int row;
		public int col;
		public coord(int r, int c) {
			row=r;
			col=c;
		}
	}
	//Cell neighbors, in anticlockwise order: 		 r-1,c+1 /  r-1,c   / r,c-1    / r+1,c-1  /  r+1,c   / r,c+1
	protected int[,] nbs = new int[,] { { -1, 1 }, { -1, 0 }, { 0, -1 }, { 1, -1 }, { 1, 0 }, { 0, 1 } };
	int winner;
	int[] nextcolor;
	int lastmove=-1;

	public HexBoard(int width=11, int height=11) : base(width,height) {
		winner = 0;
		nextcolor = new int[] { -3, 3 };
	}

	public HexBoard(HexBoard orig) : base(orig._width,orig._height) {
		for (int i = 0; i < _height; i++)
			for (int j = 0; j < _width; j++)
				board [i, j] = orig.board[i,j];	
		activeplayer = orig.activeplayer;
		movesmade = orig.movesmade;
		winner = orig.winner;
		lastmove = orig.lastmove;
		nextcolor = (int[])(orig.nextcolor.Clone ());
	}

	public override GameBoard Clone() {
		return new HexBoard (this);
	}

	public override void Reset() {
		base.Reset ();
		if (OnCellChange!=null)
			for (int i = 0; i < _height; i++)
				for (int j = 0; j < _width; j++) 
					OnCellChange (i, j,0);			
		winner = 0;
		nextcolor = new int[] { -3, 3 };
	}

	public override List<int> GetMoves() {
		List<int> output = new List<int> ();
		if (winner != 0)
			return output;
		for (int row=0;row<_height;row++)
			for (int col=0;col<_width;col++) {
				if (board [row,col] == 0)
					output.Add (row*_width+col);
			}
		return output;
	}

	public override int CheckWinner() {
		// Once you know how to efficiently compute shortest paths you can insert something nicer in here:  
		// :-)
		if (winner != 0 && OnHexWin!=null) {
			for (int row = 0; row < _height; row++)
				for (int col = 0; col < _width; col++)
					if (board [row, col] == winner || board [row, col] == 2 * winner)
						OnHexWin (row, col,winner);
		}
		return winner;
	}

	// Conventions:
	// top row = color 1
	// bottom row = color 2
	// (p1 wins iff a stone has neighbors with color 1 and 2)
	// left col = color -1
	// right col = color -2
	// (p2 wins iff a stone has neighbors with color 1 and 2)
	void Paint(coord start, sbyte oldcolor, sbyte newcolor) {
		Queue<coord> process = new Queue<coord> ();
		process.Enqueue (start);
		board [start.row, start.col] = newcolor;
		while (!(process.Count == 0)) {
			coord current = process.Dequeue ();
			for (int i = 0; i < 6; i++) {
				int nbrow = nbs [i, 0] + current.row;
				int nbcol = nbs [i, 1] + current.col;
				if (nbrow >= 0 && nbrow < _height && nbcol >= 0 && nbcol < _width) {
					if (board [nbrow, nbcol] == oldcolor) {
						//Console.WriteLine ("Recoloring {0},{1} from color {2} to {3}", nbrow, nbcol, oldcolor, newcolor);
						board [nbrow, nbcol] = newcolor;
						process.Enqueue(new coord(nbrow,nbcol));
					}
				}
			}
		}
	}

	public override string ToString() {
		string output="";
		string indent = "";
		for (int i=0;i<_height;i++) {
			output += indent;
			indent += " ";
			for (int j=0;j<_width;j++) {
				output += " " + Symbol (Math.Sign(board [i, j]))+""+Math.Abs(board[i,j]);
			}
			output+="\n";
		}
		return output;
	}

	public override string MoveToString(int move) {
		return (move / _width).ToString () + "," + (move % _width).ToString ();
	}

	public int GetLastMove() {
		return lastmove;
	}

	public override int MakeMove(int move) {
		int row = move / _width;
		int col = move % _width;
		if (board [row, col] != 0)
			return -1;
		
		lastmove = move;

		// Find all the neighboring colors:
		List<int> nbcols = new List<int> ();
		if (activeplayer == -1) {
			if (col == 0)
				nbcols.Add (-1);
			if (col == _width - 1)
				nbcols.Add (-2);
		}
		if (activeplayer == 1) {
			if (row == 0)
				nbcols.Add (1);
			if (row == _height - 1)
				nbcols.Add (2);
		}
		for (int i = 0; i < 6; i++) {
			int nbrow = nbs [i, 0] + row;
			int nbcol = nbs [i, 1] + col;
			if (nbrow >= 0 && nbrow < _height && nbcol >= 0 && nbcol < _width)
				if (board [nbrow, nbcol]*activeplayer>0) 
					nbcols.Add (board [nbrow, nbcol]); // doesn't matter that we add it twice / thrice ...
		}
		int newcol;
		if (nbcols.Contains(activeplayer))
			newcol=activeplayer;
		else if (nbcols.Contains(activeplayer*2))
			newcol=activeplayer*2;
		else if (nbcols.Count>0)
			newcol = nbcols [0];	// later: choose largest color class here, for efficiency
		else {
			newcol=nextcolor[(activeplayer+1)/2];
			nextcolor[(activeplayer+1)/2]+=activeplayer;
		}
		//Console.WriteLine ("New color: {0}",newcol);
		board [row, col] = (sbyte)newcol;		
		// notify all views of this change:
		if (OnCellChange!=null)
			OnCellChange (row, col, activeplayer);
		if (nbcols.Contains (activeplayer) && nbcols.Contains (2 * activeplayer)) {
			winner = activeplayer;
			return 0;
		}

		for (int i = 0; i < 6; i++) {
			int nbrow = nbs [i, 0] + row;
			int nbcol = nbs [i, 1] + col;
			if (nbrow >= 0 && nbrow < _height && nbcol >= 0 && nbcol < _width &&
			    board [nbrow, nbcol] * activeplayer > 0 && board [nbrow, nbcol] != newcol) {
				Paint (new coord (nbrow, nbcol), board [nbrow, nbcol], (sbyte)newcol);
				//Console.WriteLine ("Recoloring {0},{1}:\n{2}",nbrow,nbcol,ToString ());
			}
		}
		activeplayer = -activeplayer;
		movesmade++;
		return 1;
	}

	public override bool isOpeningMove()
	{
		return movesmade == 0;
	}
}
