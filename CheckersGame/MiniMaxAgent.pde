class MiniMaxAgent extends Agent
{
  final static boolean debugInfo = false;
  int _playDepth = -1;

  public MiniMaxAgent(Board pBoard, int player)
  {
    this(pBoard, player, -1);
  }

  public MiniMaxAgent(Board pBoard, int player, int pDepth)
  {
    _playBoard = pBoard;
    playerID = player;
    if (pDepth >= 0) _name = "MiniMax" + "_D" + pDepth;
    else _name = "MiniMax";
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
    if (debugInfo) println("amount of stones " + stones.size());

    for (int s = 0; s < stones.size(); ++s)
    {
      if (debugInfo) println("Evaluating stone " + s);
      _playBoard.SelectStone(stones.get(s));
      int stoneIndex = _playBoard.GetSelectedIndex();
      ArrayList<PVector> moves;
      if (hasToJump) moves = _playBoard.AvailableJumpsForStone(stones.get(s));
      else moves = _playBoard.GetMovesFor(stones.get(s));
      //cycle through all the stones
      Board clone = _playBoard.Copy();
      for (int m = 0; m < moves.size(); ++m)
      {
        float score = 0;
        clone.MakeMove(moves.get(m));
        score = getScore(clone, 0);

        if (playerID == -1) //min player
        {
          if (score < bestScore)
          {
            bestScore = score;
            bestMove = moves.get(m);
            bestStone = stones.get(s);
          }
        } else //max player
        {
          if (score > bestScore)
          {
            bestScore = score;
            bestMove = moves.get(m);
            bestStone = stones.get(s);
          }
        }
        clone.UndoLastMove();
      }
    }
    _playBoard.SelectStone(bestStone);
    return bestMove;
  }

  private int getScore(Board pBoard, int depth)
  {
    if (depth == _playDepth || pBoard.Finished())
    {
      //if game is finished or depth is reached
      return pBoard.GetScore();
    }

    boolean hasToJump = pBoard.AvailableJump() && pBoard.requiredJump;

    //holds all the stones
    int activePlayer = pBoard.GetActivePlayer();
    ArrayList<Stone> stones = pBoard.GetStones(activePlayer);
    if (debugInfo) println("amount of stones " + stones.size());

    int bestScore = 0;
    if (playerID == -1) //min player
    {
      bestScore = 200;
      for (int s = 0; s < stones.size(); ++s)
      {
        if (debugInfo) println("Evaluating stone " + s);
        pBoard.SelectStone(stones.get(s));
        int stoneIndex = pBoard.GetSelectedIndex();
        ArrayList<PVector> moves;
        if (hasToJump) moves = pBoard.AvailableJumpsForStone(stones.get(s));
        else moves = pBoard.GetMovesFor(stones.get(s));
        //cycle through all the stones
        Board clone = pBoard.Copy();
        for (int m = 0; m < moves.size(); ++m)
        {
          //Board clone = pBoard.Copy();
          //clone.MakeMove(moves.get(m));
          clone.MakeMove(moves.get(m));
          int score = getScore(pBoard, depth+1);
          if (score < bestScore)
          {
            bestScore = score;
          }
          clone.UndoLastMove();
        }
      }
    }
    else if (playerID == 1) //min player
    {
      bestScore = -200;
      for (int s = 0; s < stones.size(); ++s)
      {
        if (debugInfo) println("Evaluating stone " + s);
        pBoard.SelectStone(stones.get(s));
        int stoneIndex = pBoard.GetSelectedIndex();
        ArrayList<PVector> moves;
        if (hasToJump) moves = pBoard.AvailableJumpsForStone(stones.get(s));
        else moves = pBoard.GetMovesFor(stones.get(s));
        //cycle through all the stones
        Board clone = pBoard.Copy();
        for (int m = 0; m < moves.size(); ++m)
        {
          //Board clone = pBoard.Copy();
          //clone.MakeMove(moves.get(m));
          clone.MakeMove(moves.get(m));
          int score = getScore(pBoard, depth+1);
          if (score > bestScore)
          {
            bestScore = score;
          }
          clone.UndoLastMove();
        }
      }
    }
    return bestScore;
  }
}
