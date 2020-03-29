class Board
{
  color whiteColor = color(255,255,220);
  color blackColor = color(84,27,33);
  public ArrayList<Stone> _blackStones;
  public ArrayList<Stone> _whiteStones;
  
  public Board()
  {
    _blackStones = new ArrayList<Stone>();
    _whiteStones = new ArrayList<Stone>();
    for(int i = 0; i < 12; ++i)
    {
      int x = (i/3);
      int y = (i%3);
      PVector blackStoneTile = new PVector(x*2, y+5);
      PVector whiteStoneTile = new PVector(x*2+1, y);
      if(y%2==1)
      {
        //if y is uneven
        blackStoneTile.x += 1;
        whiteStoneTile.x-=1;
      }
      Stone black = new Stone(this, blackStoneTile, blackColor, whiteColor);
      Stone white = new Stone(this, whiteStoneTile, whiteColor, blackColor);
      _blackStones.add(black);
      _whiteStones.add(white);
    }
  }
    
  public void Render()
  {
    drawBoard(8, 8);
    for(int i = 0; i < 12; ++i)
    {
      _blackStones.get(i).Render();
      _whiteStones.get(i).Render();
    }
  }
  
  public ArrayList<Stone> GetStones(int stoneSet)
  {
    return _whiteStones;
  }
  
  private void drawBoard(int pX, int pY)
  {
    stroke(0,0,0);
    for(int y = 0; y < pY; ++y)
    {
      for(int x = 0; x < pX; ++x)
      {
        if((y+x)%2 == 0) fill(whiteColor);
        else fill(blackColor);
        rect(50+x*50, 50+y*50, 50, 50);
      }
    }
  }
  
}
