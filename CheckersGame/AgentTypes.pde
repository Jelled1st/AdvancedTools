static class AgentTypes
{
  static final int PLAYER = 0;
  static final int RANDOM = 1;
  static final int GREEDY = 2;
  static final int MONTECARLO = 3;
  static final int MONTECARLOVARIABLE = 4;
  static final int MONTECARLO80 = 5;
  static final int VERYSLOW = 6;
  static final int AMOUNT = 7;
  
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
      return "MonteCarlo";
    case AgentTypes.MONTECARLOVARIABLE:
      return "MC-VAR";
    case AgentTypes.MONTECARLO80:
      return "MC-80";
    case AgentTypes.VERYSLOW:
      return "VerySlow MiniMax";
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
  case AgentTypes.MONTECARLOVARIABLE:
    agent = new MonteCarloAgent(board, player);
    break;
  case AgentTypes.MONTECARLO80:
    agent = new MonteCarloAgent(board, player, 80);     
    break;
  case AgentTypes.VERYSLOW:
    agent = new MiniMaxAgent(board, player, 8);
    break;
  default:
    agent = new RandomAgent(board, player);
    break;
  }
  return agent;
}
