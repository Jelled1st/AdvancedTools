using GXPEngine;
using System;
using System.Collections.Generic;

class OthelloView : BoardView {
	public event CellClickHandler OnCellClick=null;

	public override void RegisterCellClickHandler(CellClickHandler newClickHandler) {
		OnCellClick += newClickHandler;
	}

	AnimationSprite[,] cell;
	List <AnimationSprite> MarkedList=new List<AnimationSprite>();
	OthelloBoard _myboard;
	List<AnimationSprite> wincells;

	public OthelloView(OthelloBoard myboard, int centerx=300, int centery=300, int targetwidth=480) {
		_myboard = myboard;

		// prepare graphics:
		cell = new AnimationSprite[myboard._height, myboard._width];
		for (int row = 0; row < _myboard._height; row++)
			for (int col = 0; col < _myboard._width; col++) {
				AnimationSprite newcell=new AnimationSprite("../../assets/tileset.png", 3, 1);
				newcell.x = col * newcell.width;
				newcell.y = row * newcell.width;
				AddChild (newcell);
				newcell.SetFrame((_myboard[row,col]+3)%3);
				cell [row, col] = newcell;
			}
		float realwidth = cell [0, 0].width * _myboard._width;
		scaleX = targetwidth/realwidth;
		scaleY = targetwidth/realwidth;
		x = centerx - targetwidth / 2;
		y = centery - targetwidth / 2; // assuming that we have a square board

		// set callbacks:
		_myboard.OnCellChange+=CellChangeHandler;
		_myboard.OnMark += MarkHandler;

		//_myboard.OnWin += WinHandler;

		wincells = new List<AnimationSprite> ();
	}

	void RemoveColor() {
		while (wincells.Count > 0) {
			wincells [0].color = 0xffffffff;
			wincells.RemoveAt (0);
		}
		while (MarkedList.Count > 0) {
			MarkedList [0].color = 0xffffffff;
			MarkedList.RemoveAt (0);
		}
	}

	public void CellChangeHandler(int row, int col, int value) {
		RemoveColor ();
		cell [row, col].SetFrame ((value + 3) % 3);
		if (value != 0) {
			cell [row, col].color = 0xffffa0a0;	// light red
			wincells.Add (cell [row, col]);
		}
	}

	public void MarkHandler(int row,int col) {
		AnimationSprite Markedcell = cell [row, col];
		Markedcell.color=0xffa0ffa0;	// light green
		MarkedList.Add (Markedcell);
	}

	public void Update() {
		if (Input.GetMouseButtonDown (0)) {
			float col = (Input.mouseX - x) / (cell [0, 0].width * scaleX);
			float row = (Input.mouseY - y) / (cell [0, 0].width * scaleY);

			if (col >= 0 && col < _myboard._width && row>=0 && row<_myboard._height) {
				Console.WriteLine ("Mouse click on column {0} and row {1}", col, row);
				// notify cellclickhandlers:
				if (OnCellClick != null)
					OnCellClick ((int)row*_myboard._width + (int)col);
			}
		}
	}

}

