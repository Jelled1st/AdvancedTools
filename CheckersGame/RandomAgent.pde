class RandomAgent extends Agent
{  
  public RandomAgent(Board pBoard, int player)
  {
    _playBoard = pBoard;
    playerID = player;
    _name = "Random";
  }
  
  public PVector MakeMove()
  {
    IntList stoneIds = new IntList();
    ArrayList<Stone> stones = _playBoard.GetStones(playerID);
    for(int i = 0 ; i < stones.size(); ++i)
    {
      stoneIds.append(i);
    }
    
    PVector move;
    
    while(true)
    {
      int random = (int)random(stoneIds.size());
      int randomStoneId = stoneIds.get(random);
      Stone stone = stones.get(randomStoneId);
      _playBoard.SelectStone(stone);
            
      ArrayList<PVector> moves = _playBoard.GetMovesFor(stone);
      if(moves.size() == 0)
      {
        stoneIds.remove(random);
        if(stoneIds.size() == 0) return new PVector(-1, -1);
        continue;
      }
      else
      {
        ArrayList<PVector> jumps = _playBoard.AvailableJumpsForStone(stone);
        if(jumps.size() == 0)
        {
          random = (int)random(moves.size());
          move = moves.get(random);
          break;
        }
        else
        {
          random = (int)random(jumps.size());
          move = jumps.get(random);
          break;
        }
      }
    }
    
    return move;
  }
}
