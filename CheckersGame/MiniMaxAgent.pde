class MiniMaxAgent extends Agent
{
  final static boolean debugInfo = false;
  
  public MiniMaxAgent(Board pBoard, int player)
  {
    _board = pBoard;
    playerID = player;
    _name = "MiniMax";
  }
  
  public void Reset()
  {
  }

  public PVector MakeMove()
  {
    return new PVector(-1,-1);
  }
}
