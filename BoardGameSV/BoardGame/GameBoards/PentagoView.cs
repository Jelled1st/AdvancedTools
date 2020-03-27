using GXPEngine;
using System;
using System.Collections.Generic;

class PentagoView : BoardView {

	//public delegate void CellClickHandler(int move);
	public event CellClickHandler OnCellClick=null;

	public override void RegisterCellClickHandler(CellClickHandler newClickHandler) {
		OnCellClick += newClickHandler;
	}

	AnimationSprite[,] cell;
	AnimationSprite[] arrow;

	PentagoBoard _myboard;
	List<AnimationSprite> wincells;

	public PentagoView(PentagoBoard myboard, int centerx=300, int centery=300, int targetwidth=480) {
		_myboard = myboard;

		// prepare graphics:
		cell = new AnimationSprite[myboard._height, myboard._width];
		for (int row = 0; row < _myboard._height; row++)
			for (int col = 0; col < _myboard._width; col++) {
				AnimationSprite newcell=new AnimationSprite("../../assets/tileset.png", 3, 1);
				newcell.x = col * newcell.width;
				newcell.y = row * newcell.width;
				AddChild (newcell);
				cell [row, col] = newcell;
			}
		float stdwidth = cell[0,0].width;
		arrow = new AnimationSprite[8];
		for (int i = 0; i < 8; i++) {
			arrow [i] = new AnimationSprite ("../../assets/arrow-sprite.png", 2, 2);
			AddChild (arrow [i]);
			arrow [i].SetScaleXY (stdwidth/arrow[i].width, stdwidth/arrow[i].width);
			arrow [i].color = 0xff000000;
		}
		/*
				0	TL CW
				1 	BL CW
				2 	TR CW
				3	BR CW
				4	TL ACW
				5 	BL ACW
				6 	TR ACW
				7	BR ACW
				*/
		arrow [0].x = stdwidth * 1;
		arrow [0].y = stdwidth * -1;
		arrow [0].SetFrame (1);
		arrow [1].x = stdwidth * -1;
		arrow [1].y = stdwidth * 4;
		arrow [1].SetFrame (2);
		arrow [2].x = stdwidth * 6;
		arrow [2].y = stdwidth * 1;
		arrow [2].SetFrame (3);
		arrow [3].x = stdwidth * 4;
		arrow [3].y = stdwidth * 6;
		arrow [3].SetFrame (0);

		arrow [4].x = stdwidth * -1;
		arrow [4].y = stdwidth * 1;
		arrow [4].SetFrame (3);
		arrow [5].x = stdwidth * 1;
		arrow [5].y = stdwidth * 6;
		arrow [5].SetFrame (1);
		arrow [6].x = stdwidth * 4;
		arrow [6].y = stdwidth * -1;
		arrow [6].SetFrame (0);
		arrow [7].x = stdwidth * 6;
		arrow [7].y = stdwidth * 4;
		arrow [7].SetFrame (2);


		float realwidth = stdwidth * 8;
		scaleX = targetwidth/realwidth;
		scaleY = targetwidth/realwidth;
		x = centerx - targetwidth / 2 + scaleX*stdwidth;
		y = centery - targetwidth / 2 + scaleX*stdwidth;

		// set callbacks:
		_myboard.OnCellChange+=CellChangeHandler;

		_myboard.OnWin += WinHandler;

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
		for (int i = 0; i < 8; i++) {
			if (_myboard.GetTurn())
				arrow [i].color = 0xff000000;
			else
				arrow [i].color = 0xffffffff;
		}
	}

	public void WinHandler(int startrow, int startcol, int rowdir, int coldir, int length) {
		//RemoveColor ();
		for (int i = 0; i < length; i++) {
			AnimationSprite wincell = cell [startrow + rowdir * i, startcol + coldir * i];
			wincell.color = 0xffa0ffa0;  // light green
			wincells.Add (wincell);
		}
	}


	public void Update() {
		if (Input.GetMouseButtonDown (0)) {
			float col = ((Input.mouseX - x) / (cell [0, 0].width * scaleX));
			float row = ((Input.mouseY - y) / (cell [0, 0].width * scaleY));

			if (OnCellClick != null) {
				if (col >= 0 && col < _myboard._width && row >= 0 && row < _myboard._height) {
					Console.WriteLine ("Mouse click on column {0} and row {1}", col, row);
					// notify cellclickhandlers:
					OnCellClick ((int)col + ((int)row) * _myboard._width);
				} else {
					
					for (int i = 0; i < 8; i++)
						if (arrow[i].HitTestPoint(Input.mouseX,Input.mouseY)) {
						//if (Input.mouseX - x >= arrow [i].x && Input.mouseX - x <= arrow [i].x + arrow [i].width &&
						//    Input.mouseY - y >= arrow [i].y && Input.mouseY - y <= arrow [i].y + arrow [i].height) {
							Console.WriteLine ("Click on arrow {0}", i);
							OnCellClick (i + 36);
						}
					

							/*
					Console.WriteLine ("Mouse click on column {0} and row {1} (TURN)", col, row);

					if (row == -1 && col == 1)
						OnCellClick (36);
					if (row == 4 && col == -1)
						OnCellClick (37);
					if (row == 6 && col == 2)
						OnCellClick (38);
					if (row == 6 && col == 4)
						OnCellClick (39);
					if (row == 1 && col == -1)
						OnCellClick (40);
					if (row == 1 && col == 6)
						OnCellClick (41);
					if (row == -1 && col == 4)
						OnCellClick (42);
					if (row == 6 && col == 4)
						OnCellClick (43);
				*/

				}
			}
		}
	}

}

