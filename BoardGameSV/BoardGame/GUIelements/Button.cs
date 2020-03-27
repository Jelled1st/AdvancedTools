using GXPEngine;
using System.Drawing;
using System;

// Displays a message, responds to clicks (register your handler to the OnClick event).
class Button : Canvas { 
	public Color backgroundColor=Color.Black;
	public Color textColor=Color.Brown;
	Font font; 
	bool active=true;

	public delegate void ButtonHandler();
	public event ButtonHandler OnClick=null;

	public Button(int width, int height, int pX, int pY, string starttext="", Font pFont=null) : base(width,height) {
		x = pX;
		y = pY;
		if (pFont == null)
			font = new Font ("Verdana", 16);
		else
			font = pFont;
		ShowMessage (starttext);
	}

	public void ShowMessage(string newtext) {
		graphics.Clear (backgroundColor);
		Brush textBrush;
		textBrush = new SolidBrush (textColor);
		graphics.DrawString(newtext,font,textBrush,0,0);
	}

	public void Update() {
		if (active && Input.GetMouseButtonDown(0) && HitTestPoint (Input.mouseX, Input.mouseY) && OnClick!=null) {
			OnClick ();
		}
	}
}

