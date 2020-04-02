static class AgentTypes
{
  static final int PLAYER = 0;
  static final int RANDOM = 1;
  static final int MONTECARLO = 2;
  static final int MONTECARLO1000 = 3;
  static final int AMOUNT = 4;
  
  static public String ToString(int type)
  {
    switch(type)
    {
    case AgentTypes.PLAYER:
      return "Player";
    case AgentTypes.RANDOM:
      return "Random";
    case AgentTypes.MONTECARLO:
      return "MonteCarlo";
    case AgentTypes.MONTECARLO1000:
      return "M-C-1000";
    default:
      return "Not found";
    }
  }
}

Agent GetAgent(int type, Board board, int player)
{
  Agent agent = null;
  switch(type)
  {
  case AgentTypes.PLAYER:
    agent = new PlayerAgent(board, player);
    break;
  case AgentTypes.RANDOM:
    agent = new RandomAgent(board, player);
    break;
  case AgentTypes.MONTECARLO:
    agent = new MonteCarloAgent(board, player);
    break;
  case AgentTypes.MONTECARLO1000:
    agent = new MonteCarloAgent(board, player, 1000);     
    break;
  default:
    agent = new RandomAgent(board, player);
    break;
  }
  return agent;
}
