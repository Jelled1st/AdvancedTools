class PlayerAgent extends Agent
{
  public PlayerAgent(Board pBoard, int player)
  {
    _playBoard = pBoard;
    playerID = player;
    _name = "Player";
  }
  
  public PVector MakeMove()
  {
    ArrayList<Stone> stones = _playBoard.GetStones(playerID);
    for(int i = 0; i < stones.size(); ++i)
    {
      if(stones.get(i).IsClicked())
      {
        _playBoard.SelectStone(stones.get(i));
      }
    }
    if(mousePressed)
    {
      PVector tile = _playBoard.GetTileAtPosition(new PVector(mouseX, mouseY));
      Stone selected = _playBoard.GetSelectedStone();
      if(selected != null)
      {
        PVector stoneTile = selected.GetTile();
        return tile;
      }
    }
    return null;
  }
}
