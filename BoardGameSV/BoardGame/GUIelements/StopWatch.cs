using GXPEngine;
using System.Drawing;
using System;

// A stopwatch (=chess clock) GUI element.
class StopWatch : Canvas { 
	public delegate void TimeOutHandler();
	public event TimeOutHandler OnTimeOut=null;	// Called when the time is up

	public Color backgroundColor=Color.Black;
	public Color textColor=Color.Brown;
	bool active=false;
	int currentTime;

	Font font; 

	public StopWatch(int width, int height, int pX, int pY, int pStartTime, Font pFont=null) : base(width,height) {
		x = pX;
		y = pY;
		if (pFont == null)
			font = new Font ("Verdana", 16);
		else
			font = pFont;
		currentTime = pStartTime;
		Redraw ();
	}

	void Redraw() {
		graphics.Clear (backgroundColor);
		graphics.DrawRectangle (new Pen(Color.Black),0, 0, width-1, height-1);
		Brush textBrush;
		textBrush = new SolidBrush (textColor);
		string timestring=(currentTime/60000).ToString()+":"+((currentTime/10000)%6).ToString()+((currentTime/1000)%10).ToString();
		graphics.DrawString(timestring,font,textBrush,0,0);
	}

	public void SetActive(bool pActive) {
		active=pActive;	
		if (active)
			textColor = Color.Red;
		else
			textColor = Color.Brown;
		Redraw ();
	}

	public void SetTime(int newtime) {
		currentTime = newtime;
		Redraw ();
	}

	public int GetTime() {
		return currentTime;
	}

	public void Update() {
		if (active) {
			currentTime -= Time.deltaTime;
			if (currentTime < 0) {
				currentTime = 0;
				active = false;
				if (OnTimeOut != null)
					OnTimeOut ();
			}
			Redraw ();
		}
	}
}

