using GXPEngine;
using System;

abstract class BoardView : GameObject {
	public delegate void CellClickHandler(int move);

	public abstract void RegisterCellClickHandler(CellClickHandler newClickHandler);
}
