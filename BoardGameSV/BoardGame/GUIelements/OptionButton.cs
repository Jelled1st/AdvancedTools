using GXPEngine;
using System.Drawing;
using System;

// A button with a set of options that can be selected by right- or left-clicking on the button. Displays the selected option.
class OptionButton : Canvas { 
	public delegate void ClickHandler(int selection);
	public event ClickHandler OnButtonClick=null;

	string[] options;
	int selected=0;
	public Color backgroundColor = Color.FromArgb (64, 0, 0, 0);//  Black;
	public Color textColor=Color.White;
	bool active=true;

	Font font; 

	public OptionButton(int width, int height, int pX, int pY, string[] pOptions, Font pFont=null) : base(width,height) {
		options=(string[])pOptions.Clone();
		x = pX;
		y = pY;
		if (pFont == null)
			font = new Font ("Verdana", 16);
		else
			font = pFont;
		Redraw ();
	}

	void Redraw() {
		graphics.Clear (backgroundColor);
		graphics.DrawRectangle (new Pen(Color.Black),0, 0, width-1, height-1);
		Brush textBrush;
		textBrush = new SolidBrush (textColor);
		graphics.DrawString(options[selected],font,textBrush,0,0);
	}

	public void SetActive(bool pActive) {
		active=pActive;	
		if (active)
			alpha = 1;
		else
			alpha = 0.5f;
		Redraw ();
	}

	public int GetSelection() {
		return selected;
	}

	public void SetSelection(int choice) {
		selected=choice;
		Redraw();
	}

	public string GetSelectionString() {
		return options [selected];
	}

	public void Update() {
		if (active && HitTestPoint (Input.mouseX, Input.mouseY)) {
			bool change = false;
			if (Input.GetMouseButtonDown (1)) {
				selected = (selected + 1) % options.Length;
				change = true;
			}
			if (Input.GetMouseButtonDown (0)) {
				selected = (selected + options.Length - 1) % options.Length;
				change=true;
			}
			if (change) {
				Redraw();
				if (OnButtonClick != null)
					OnButtonClick (selected);
			}
		}
	}
}
