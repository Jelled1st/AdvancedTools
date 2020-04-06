static class AgentTypes
{
  static final int PLAYER = 0;
  static final int RANDOM = 1;
  static final int MONTECARLO = 2;
  static final int MONTECARLOTESTS = 3;
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
    case AgentTypes.MONTECARLOTESTS:
      return "MC-T";
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
  case AgentTypes.MONTECARLOTESTS:
    agent = new MonteCarloAgent(board, player, 100);     
    break;
  default:
    agent = new RandomAgent(board, player);
    break;
  }
  return agent;
}
