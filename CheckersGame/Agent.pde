abstract class Agent
{
  protected Board _playBoard;
  protected int playerID;
  protected String _name = "Name";
  
  abstract public PVector MakeMove();
  
  //returns player as 1 and 2
  protected int getPlayerPositive()
  {
    float inBetween = playerID / 2.0f;
    int player = (int)(inBetween + 1.5f);
    return player;
  }
  
  public String Name()
  {
    return _name;
  }
  
  public void Reset()
  {
    return;
  }
}
