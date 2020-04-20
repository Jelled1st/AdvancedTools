static class AgentTypes
{
  static final int PLAYER = 0;
  static final int RANDOM = 1;
  static final int GREEDY = 2;
  static final int MONTECARLO = 3;
  static final int ALPHABETA = 4;
  static final int ALPHA_AGENT = 5;
  static final int AMOUNT = 6;
  
  static public String ToString(int type)
  {
    switch(type)
    {
    case AgentTypes.PLAYER:
      return "Player";
    case AgentTypes.RANDOM:
      return "Random";
    case AgentTypes.GREEDY:
      return "Greedy";
    case AgentTypes.MONTECARLO:
      return "MC-VAR";
    case AgentTypes.ALPHABETA:
      return "AlphaBeta";
    case AgentTypes.ALPHA_AGENT:
      return "Alpga";
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
  case AgentTypes.GREEDY:
    agent = new GreedyAgent(board, player);
    break;
  case AgentTypes.MONTECARLO:
    agent = new MonteCarloAgent(board, player);
    break;
  case AgentTypes.ALPHABETA:
    agent = new AlphaBetaPruningAgent(board, player, -1);
    break;
  case AgentTypes.ALPHA_AGENT:
    agent = new AlphaAgent(board, player, -1);
    break;
  default:
    agent = new RandomAgent(board, player);
    break;
  }
  return agent;
}
