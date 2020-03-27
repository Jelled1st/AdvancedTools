using GXPEngine;
using System;
using System.Collections.Generic;

class HexView : BoardView {

	//public delegate void CellClickHandler(int move);
	public event CellClickHandler OnCellClick=null;

	public override void RegisterCellClickHandler(CellClickHandler newClickHandler) {
		OnCellClick += newClickHandler;
	}

	AnimationSprite[,] cell;
	AnimationSprite[] indicator;
	HexBoard _myboard;
	List<AnimationSprite> wincells;

	bool win=false;

	public HexView(HexBoard myboard, int centerx=300, int centery=300, int targetwidth=480) {
		_myboard = myboard;

		// prepare graphics:
		cell = new AnimationSprite[myboard._height, myboard._width];
		for (int row = 0; row < _myboard._height; row++)
			for (int col = 0; col < _myboard._width; col++) {
				AnimationSprite newcell=new AnimationSprite("../../assets/tileset.png", 3, 1);
				newcell.x = (col + 0.5f * row) * newcell.width;
				newcell.y = row * newcell.width;
				AddChild (newcell);
				cell [row, col] = newcell;
			}
		float stdwidth = cell [0, 0].width;

		float realwidth = stdwidth * (_myboard._width) * 1.5f;
		scaleX = targetwidth/realwidth;
		scaleY = targetwidth/realwidth;
		x = centerx - targetwidth / 2;
		y = centery - targetwidth / 3; // assuming that we have a square board

		indicator = new AnimationSprite[4];
		for (int i = 0; i < 4; i++) {
			indicator[i]=new AnimationSprite("../../assets/tileset.png", 3, 1);
			AddChild (indicator[i]);
		}
		indicator[0].x = 0;
		indicator[0].y = stdwidth*myboard._height / 2f;
		indicator[0].SetFrame (2);
		indicator[1].x = (myboard._width-1) * stdwidth * 1.5f;
		indicator[1].y = stdwidth * (myboard._height-2) / 2f;
		indicator[1].SetFrame (2);
		indicator[2].x = realwidth / 2 - 2.5f*stdwidth; 
		indicator[2].y = -stdwidth * 1.5f;
		indicator[2].SetFrame (1);
		indicator[3].x = realwidth / 2 + stdwidth; 
		indicator[3].y = realwidth*2/3 + stdwidth * 0.5f;
		indicator[3].SetFrame (1);

		// set callbacks:
		_myboard.OnCellChange+=CellChangeHandler;
		_myboard.OnHexWin += WinHandler;

		wincells = new List<AnimationSprite> ();
	}

	void RemoveColor() {
		while (wincells.Count > 0) {
			wincells [0].color = 0xffffffff;
			wincells.RemoveAt (0);
		}
	}

	public void CellChangeHandler(int row, int col, int value) {
		RemoveColor ();
		cell [row, col].SetFrame ((value + 3) % 3);
		if (value != 0) {
			cell [row, col].color = 0xffffa0a0;	// light red
			wincells.Add (cell [row, col]);
		}
		win = false;
	}

	public void WinHandler(int startrow, int startcol, int winner) {
		if (!win) {
			RemoveColor ();
			win = true;
			if (winner == -1) {
				indicator[0].color=0xffa0ffa0;
				wincells.Add (indicator[0]);
				indicator[1].color=0xffa0ffa0;
				wincells.Add (indicator[1]);
			} else {
				indicator[2].color=0xffa0ffa0;
				wincells.Add (indicator[2]);
				indicator[3].color=0xffa0ffa0;
				wincells.Add (indicator[3]);
			}
		}
		AnimationSprite wincell = cell [startrow, startcol];
		wincell.color = 0xffa0ffa0;  // light green
		wincells.Add (wincell);
	}


	public void Update() {
		if (Input.GetMouseButtonDown (0)) {
			float row = (Input.mouseY - y) / (cell [0, 0].width * scaleY);
			float col = (Input.mouseX - x - (int)row*cell[0,0].width*scaleX/2) / (cell [0, 0].width * scaleX);

			if (col >= 0 && col < _myboard._width && row>=0 && row<_myboard._height) {
				Console.WriteLine ("Mouse click on column {0} and row {1}", col, row);
				// notify cellclickhandlers:
				if (OnCellClick != null)
					OnCellClick ((int)col+((int)row)*_myboard._width);
			}
		}
	}

}


