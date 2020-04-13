class MonteCarloAgent extends Agent
{
  final static boolean debugInfo = false;
  int _seeds[];
  int _samples;
  int _playDepth;
  
  IntList playedDepths;
  int playedGames = 0;
  
  public MonteCarloAgent(Board pBoard, int player)
  {
    this(pBoard, player, 25, -1);
  }

  public MonteCarloAgent(Board pBoard, int player, int pSamples)
  {
    this(pBoard, player, pSamples, -1);
  }

  public MonteCarloAgent(Board pBoard, int player, int pSamples, int pDepth)
  {
    _playBoard = pBoard;
    playerID = player;
    _playDepth = pDepth;
    _samples = pSamples;
    _name = "MCarlo_" + _samples;
    if(pDepth != -1) _name += "D" + pDepth;
    _seeds = new int[_samples];
    for(int i = 0; i < _samples; ++i) 
    {
      _seeds[i] = (int)random(1000000);
    }
  }
  
  public void SetSeed(int index, int pSeed)
  {
    if(index < 0 || index >= _samples) return;
    _seeds[index] = pSeed;
  }
  
  public void Reset()
  {
    _seeds = new int[_samples];
    for(int i = 0; i < _samples; ++i) 
    {
      _seeds[i] = (int)random(1000000);
    }
  }

  public PVector MakeMove()
  {
    playedDepths = new IntList();
    playedGames = 0;
    
    //int seed = (int)random(1000000);
    float bestScore = -200; //lower than the lowest score possible
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
        if(debugInfo) println("Evaluating move " + m + " out of " + moves.size());
        int wins = 0;
        int losses = 0;
        //cycle through all the moves for each stone
        //randomSeed(seed);
        for (int i = 0; i < _samples; ++i)
        {
          Board copy = _playBoard.Copy();
          copy.SelectStone(stoneIndex); //this should select the 'same' stone on the copy board
          copy.MakeMove(moves.get(m)); //make the move
          int winner = PlayRandomGame(copy, _seeds[i], _playDepth);
          if (winner == playerID) ++wins;
          else if (winner == -playerID) ++losses;
        }
        if(debugInfo) println("done with samples");
        float score = (wins - losses) / (float)_samples;
        //float score = (losses - wins) / (float)_samples;
        if (score > bestScore)
        {
          if(debugInfo) println("new highscore, Score: " + score);
          bestScore = score;
          bestMove = moves.get(m);
          bestStone = stones.get(s);
        } else if(debugInfo) println("lower score, score was: " + score);
      }
    }
    float averageDepth = 0;
    for(int i = 0; i < playedDepths.size(); ++i)
    {
      averageDepth += playedDepths.get(i);
    }
    averageDepth /= playedGames;
    if(debugInfo) println("Played a total of " + playedGames + " games with avarage depth: " + averageDepth);
    
    _playBoard.SelectStone(bestStone);
    return bestMove;
  }
  
  private int PlayRandomGame(Board pCopy, int seed)
  {
    return PlayRandomGame(pCopy, seed, -1);
  }
  
  private int PlayRandomGame(Board pCopy, int seed, int pDepth)
  {
    if(debugInfo) 
    {
      println("\n------------------------------\n" +
              "Random play\n" + 
              "Attributes:\n" +
              "\tDepth: " + pDepth + "\n" +
              "------------------------------\n");
    }
    randomSeed(seed);
    int depth = 0;
    //while the game is not finished and depth has not been reached 
    while(!(pCopy.Finished() || depth == pDepth))
    {
      if(debugInfo) println("\tEntering board.finished() loop");
      boolean madeMove = false;
      
      ArrayList<Stone> stones = pCopy.GetStones(pCopy.GetActivePlayer());
      //this while is to get a stone and a move
      //it may however occur that a stone cannot make a move and that a new stone needs be chosen
      while(true)
      {
        if(debugInfo) println("\t\tEntering pick stone loop, stones.size() " + stones.size());
        if(stones.size() == 0)
        {
          //if there are no stones left to pick from
          //there are no legit moves
          //there fore the game would be finished
          return pCopy.CheckWinner();
        }
        int randomStoneId = (int)random(stones.size());
        Stone stone = stones.get(randomStoneId);
      
        ArrayList<PVector> moves = pCopy.GetMovesFor(stone);
        if(moves.size() == 0)
        {
          stones.remove(randomStoneId);
          continue;
        }
        else
        {
          pCopy.SelectStone(stone);
          //this wile is to choose a move for the stone
          //just like with the stone, it may occur that a move cannot be made
          //and a new move needs to be chosen
          ArrayList<PVector> jumps = pCopy.AvailableJumpsForStone(stone);
          while(true)
          {
            if(debugInfo) println("\t\t\tEntering pick move loop, stone: " + randomStoneId + ", moves: " + moves.size() + ", jumps: " + jumps.size());
            if(moves.size() == 0)
            {
              //within this loop moves may be removed from the list
              //if there are no available moves left
              //exit this loop
              break;
            }
            
            if(jumps.size() == 0)
            {
              //make a normal move
              int randomMoveId = (int)random(moves.size());
              PVector move = moves.get(randomMoveId);
              madeMove = pCopy.MakeMove(move);
              if(debugInfo) println("\t\t\tTried move: " + madeMove);
              if(!madeMove)
              {
                moves.remove(randomMoveId);
              }
            }
            else
            {
              //make a jump move
              int randomJumpId = (int)random(jumps.size());
              PVector move = jumps.get(randomJumpId);
              madeMove = pCopy.MakeMove(move);
              if(debugInfo) println("\t\t\tTried move: " + madeMove);
              if(!madeMove)
              {
                jumps.remove(randomJumpId);
              }
              
            } //end of if-else // normal move or jump
            
            if(madeMove)
            {
              ++depth;
              break;
            }
          } //end of while // trying to apply the move
          
          if(moves.size() == 0)
          {
            stones.remove(randomStoneId);
            continue;
          }
        } //end of if-else // moves == 0
        
        if(madeMove) break;
      } //end of while // picking a random stone
      
    } //end of while loop // board.finished
    if(debugInfo) println("Exited with depth: " + depth);
    playedDepths.append(depth);
    ++playedGames;
    return pCopy.CheckWinner();
  }
}
