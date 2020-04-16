class AlphaBetaPruningAgent extends Agent
{
  final static boolean debugInfo = false;
  int _playDepth = -1;
  
  public AlphaBetaPruningAgent(Board pBoard, int player)
  {
    this(pBoard, player, -1);
  }
  
  public AlphaBetaPruningAgent(Board pBoard, int player, int pDepth)
  {
    _playBoard = pBoard;
    playerID = player;
    if(pDepth >= 0) _name = "AlphaBeta" + "_D" + pDepth;
    else _name = "AlphaBeta";
    _playDepth = pDepth;
  }

  public PVector MakeMove()
  {
    float bestScore = -(playerID)*200; //lower than the lowest score possible
    PVector bestMove = new PVector(-1, -1);
    Stone bestStone = null;
    
    boolean hasToJump = _playBoard.AvailableJump() && _playBoard.requiredJump;

    //holds all the stones
    ArrayList<Stone> stones = _playBoard.GetStones(_playBoard.GetActivePlayer());
    if(debugInfo) println("amount of stones " + stones.size());

    for (int s = 0; s < stones.size(); ++s)
    {
      if(debugInfo) println("Evaluating stone " + s);
      _playBoard.SelectStone(stones.get(s));
      int stoneIndex = _playBoard.GetSelectedIndex();
      ArrayList<PVector> moves;
      if(hasToJump) moves = _playBoard.AvailableJumpsForStone(stones.get(s));
      else moves = _playBoard.GetMovesFor(stones.get(s));
      //cycle through all the stones
      for (int m = 0; m < moves.size(); ++m)
      {
        Board clone = _playBoard.Copy();
        clone.MakeMove(moves.get(m));
        int score = getScore(clone, 0, -200, 200);
        
        if(playerID == -1) //min player
        {
          if(score < bestScore)
          {
            bestScore = score;
            bestMove = moves.get(m);
            bestStone = stones.get(s);
          }
        }
        else //max player
        {
          if(score > bestScore)
          {
            bestScore = score;
            bestMove = moves.get(m);
            bestStone = stones.get(s);
          }
        }
      }
    }
    _playBoard.SelectStone(bestStone);
    return bestMove;
  }
  
  private int getScore(Board pBoard, int depth, int alpha, int beta)
  {
    int winner = pBoard.CheckWinner();
    if(depth == _playDepth || winner != 0 || pBoard.Finished())
    {
      //if game is finished or depth is reached
      return pBoard.GetScore();
    }
    
    boolean hasToJump = pBoard.AvailableJump() && pBoard.requiredJump;

    //holds all the stones
    int activePlayer = pBoard.GetActivePlayer();
    ArrayList<Stone> stones = pBoard.GetStones(activePlayer);
    if(debugInfo) println("amount of stones " + stones.size());

    int bestScore = -(activePlayer)*200; //set score to lowest possible

    for (int s = 0; s < stones.size(); ++s)
    {
      if(debugInfo) println("Evaluating stone " + s);
      pBoard.SelectStone(stones.get(s));
      int stoneIndex = pBoard.GetSelectedIndex();
      ArrayList<PVector> moves;
      if(hasToJump) moves = pBoard.AvailableJumpsForStone(stones.get(s));
      else moves = pBoard.GetMovesFor(stones.get(s));
      //cycle through all the stones
      for (int m = 0; m < moves.size(); ++m)
      {
        pBoard.MakeMove(moves.get(m));
        int score = 0;
        score = getScore(pBoard, depth+1, alpha, beta);
        
        if(playerID == -1) //min player
        {
          if(score < bestScore)
          {
            bestScore = score;
          }
          
          beta = min(beta, score);
          if(beta <= alpha)
          {
            pBoard.UndoLastMove();
            break;
          }
        }
        else //max player
        {
          if(score > bestScore)
          {
            bestScore = score;
          }
          alpha = max(alpha, score);
          if(beta <= alpha)
          {
            pBoard.UndoLastMove();
            break;
          }
        }
        pBoard.UndoLastMove();
      }
    }
    return bestScore;
  }
}
