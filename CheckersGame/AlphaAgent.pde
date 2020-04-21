class AlphaAgent extends Agent
{
  final static boolean debugInfo = false;
  int _playDepth = -1;
  int _samples;
  boolean _quiescenceSearch = false; 

  public AlphaAgent(Board pBoard, int player)
  {
    this(pBoard, player, -1);
  }

  public AlphaAgent(Board pBoard, int player, int pDepth)
  {
    this(pBoard, player, pDepth, 25);
  }

  public AlphaAgent(Board pBoard, int player, int pDepth, int pSampleCount)
  {
    _playBoard = pBoard;
    playerID = player;
    if (pDepth >= 0) _name = "Alpha" + "_D" + pDepth + "_S" + pSampleCount;
    else _name = "Alpha" + "_S" + pSampleCount;
    _playDepth = pDepth;
    _samples = pSampleCount;
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
      for (int m = 0; m < moves.size(); ++m)
      {
        Board clone = _playBoard.Copy();
        clone.MakeMove(moves.get(m));
        float score = getScore(clone, 0, -200, 200);

        if (playerID == -1) //min player
        {
          if (score < bestScore)
          {
            bestScore = score;
            bestMove = moves.get(m);
            bestStone = stones.get(s);
          }
        } 
        else //max player
        {
          if (score > bestScore)
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

  //depth is the current depth of the search/tree
  //alpha is the highest found value yet
  //beta is the lowest found value yet
  private float getScore(Board pBoard, int depth, float alpha, float beta)
  {
    if (depth >= _playDepth || pBoard.Finished())
    {
      //if game is finished or depth is reached
      if (pBoard.Finished()) 
      {
        //if there is a winner return that winner
        return pBoard.CheckWinner() * 2; //this counts as a higher win, since it is sure
      }

      int activePlayerBeforeJumps = pBoard.GetActivePlayer();
  
      //if there is still a jump left to do, make the jump
      //it can make up to 10 more jumps
      int totalStones = pBoard.GetStones(-1).size() + pBoard.GetStones(1).size();
      int whileLoops = 0;
      while (_quiescenceSearch && whileLoops <= 10 && pBoard.AvailableJump())
      {
        if(debugInfo) println("A jump is available: " + whileLoops);
        //compile another move
        int activePlayer = pBoard.GetActivePlayer();
        ArrayList<Stone> stones = pBoard.GetStones(activePlayer);
        if (debugInfo) println("amount of stones " + stones.size());
        for (int s = 0; s < stones.size(); ++s)
        {
          if(debugInfo) println("Evaluating stone " + s);
          pBoard.SelectStone(stones.get(s));
          int stoneIndex = pBoard.GetSelectedIndex();
          ArrayList<PVector> moves;
          moves = pBoard.AvailableJumpsForStone(stones.get(s));
          if(moves == null || moves.size() == 0) continue;
          int randomMove = (int)random(moves.size());
          pBoard.MakeMove(moves.get(randomMove));
          break;
        }
        ++whileLoops;
      }
      if(whileLoops > 0 && debugInfo) println("Exiting jumps!"); 

      //continue with Monte Carlo
      int wins = 0;
      int losses = 0;
      for (int i = 0; i < _samples; ++i)
      {
        Board copy = pBoard.Copy();
        int outcome = PlayRandomGame(copy);
        if (outcome == activePlayerBeforeJumps) ++wins;
        else if (outcome == -activePlayerBeforeJumps) ++losses;
      }
      float score = (wins - losses) / (float)_samples;
      
      
      while (whileLoops > 0)
      {
        pBoard.UndoLastMove();
        --whileLoops;
      }
      int stonesAfterUndos = pBoard.GetStones(-1).size() + pBoard.GetStones(1).size();
      if(stonesAfterUndos != totalStones) println("Stones after undos is NOT correct: first(" + totalStones + "), after(" + stonesAfterUndos + ")");
      
      return score;
    }

    boolean hasToJump = pBoard.AvailableJump() && pBoard.requiredJump;

    //holds all the stones
    int activePlayer = pBoard.GetActivePlayer();
    ArrayList<Stone> stones = pBoard.GetStones(activePlayer);
    if (debugInfo) println("amount of stones " + stones.size());

    float bestScore = 0; //set score to lowest possible

    if (activePlayer == -1)
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
        
        Board board = pBoard.Copy();
        for (int m = 0; m < moves.size(); ++m)
        {
          board.MakeMove(moves.get(m));
          float score = 0;
          score = getScore(board, depth+1, alpha, beta);

          if (score < bestScore)
          {
            bestScore = score;
          }

          beta = min(beta, bestScore);
          if (beta <= alpha)
          {
            board.UndoLastMove();
            break;
          }

          board.UndoLastMove();
        }
      }
    } else if (activePlayer == 1)
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

        Board board = pBoard.Copy();
        for (int m = 0; m < moves.size(); ++m)
        {
          board.MakeMove(moves.get(m));
          float score = 0;
          score = getScore(board, depth+1, alpha, beta);

          if (score > bestScore)
          {
            bestScore = score;
          }

          alpha = max(beta, bestScore);
          if (beta <= alpha)
          {
            board.UndoLastMove();
            break;
          }
          board.UndoLastMove();
        }
      }
    }
    return bestScore;
  }


  private int PlayRandomGame(Board pCopy)
  {
    //while the game is not finished and depth has not been reached 
    while (!pCopy.Finished())
    {
      if (debugInfo) println("\tEntering board.finished() loop");
      boolean madeMove = false;

      ArrayList<Stone> stones = pCopy.GetStones(pCopy.GetActivePlayer());
      //this while is to get a stone and a move
      //it may however occur that a stone cannot make a move and that a new stone needs be chosen
      while (true)
      {
        if (debugInfo) println("\t\tEntering pick stone loop, stones.size() " + stones.size());
        if (stones.size() == 0)
        {
          //if there are no stones left to pick from
          //there are no legit moves
          //there fore the game would be finished
          return pCopy.CheckWinner();
        }
        int randomStoneId = (int)random(stones.size());
        Stone stone = stones.get(randomStoneId);

        ArrayList<PVector> moves = pCopy.GetMovesFor(stone);
        if (moves.size() == 0)
        {
          stones.remove(randomStoneId);
          continue;
        } else
        {
          pCopy.SelectStone(stone);
          //this wile is to choose a move for the stone
          //just like with the stone, it may occur that a move cannot be made
          //and a new move needs to be chosen
          ArrayList<PVector> jumps = pCopy.AvailableJumpsForStone(stone);
          while (true)
          {
            if (debugInfo) println("\t\t\tEntering pick move loop, stone: " + randomStoneId + ", moves: " + moves.size() + ", jumps: " + jumps.size());
            if (moves.size() == 0)
            {
              //within this loop moves may be removed from the list
              //if there are no available moves left
              //exit this loop
              break;
            }

            if (jumps.size() == 0)
            {
              //make a normal move
              int randomMoveId = (int)random(moves.size());
              PVector move = moves.get(randomMoveId);
              madeMove = pCopy.MakeMove(move);
              if (debugInfo) println("\t\t\tTried move: " + madeMove);
              if (!madeMove)
              {
                moves.remove(randomMoveId);
              }
            } else
            {
              //make a jump move
              int randomJumpId = (int)random(jumps.size());
              PVector move = jumps.get(randomJumpId);
              madeMove = pCopy.MakeMove(move);
              if (debugInfo) println("\t\t\tTried move: " + madeMove);
              if (!madeMove)
              {
                jumps.remove(randomJumpId);
              }
            } //end of if-else // normal move or jump

            if (madeMove)
            {
              break;
            }
          } //end of while // trying to apply the move

          if (moves.size() == 0)
          {
            stones.remove(randomStoneId);
            continue;
          }
        } //end of if-else // moves == 0

        if (madeMove) break;
      } //end of while // picking a random stone
    } //end of while loop // board.finished
    return pCopy.CheckWinner();
  }
}
