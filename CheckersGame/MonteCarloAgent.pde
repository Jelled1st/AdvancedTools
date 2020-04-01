class MonteCarloAgent extends Agent
{
  boolean debugInfo = false;
  
  int _samples;
  public MonteCarloAgent(Board pBoard, int player)
  {
    _playBoard = pBoard;
    playerID = player;
    _samples = 25;
    _name = "M-Carlo 25";
  }

  public MonteCarloAgent(Board pBoard, int player, int pSamples)
  {
    _playBoard = pBoard;
    playerID = player;
    _samples = pSamples;
    _name = "M-Carlo " + _samples;
  }

  public PVector MakeMove()
  {
    float bestScore = -abs(playerID)*2; //lower than the lowest score possible
    PVector bestMove = new PVector(-1, -1);
    Stone bestStone = null;
    
    boolean hasToJump = _playBoard.AvailableJump();

    //holds all the stones
    ArrayList<Stone> stones = _playBoard.GetStones(_playBoard.GetActivePlayer());
    if(debugInfo) println("amount of stones " + stones.size());

    for (int s = 0; s < stones.size(); ++s)
    {
      println("Evaluating stone " + s);
      _playBoard.SelectStone(stones.get(s));
      int stoneIndex = _playBoard.GetSelectedIndex();
      ArrayList<PVector> moves;
      if(hasToJump) moves = _playBoard.AvailableJumpsForStone(stones.get(s));
      else moves = _playBoard.GetMovesFor(stones.get(s));
      //cycle through all the stones
      for (int m = 0; m < moves.size(); ++m)
      {
        println("Evaluating move " + m + " out of " + moves.size());
        int wins = 0;
        int losses = 0;
        //cycle through all the moves for each stone
        for (int i = 0; i < _samples; ++i)
        {
          Board copy = _playBoard.Copy();
          copy.SelectStone(stoneIndex); //this should select the 'same' stone on the copy board
          copy.MakeMove(moves.get(m)); //make the move
          int winner = PlayRandomGame(copy);
          if (winner == playerID) ++wins;
          else if (winner == -playerID) ++losses;
        }
        println("done with samples");
        float score = (losses - wins) / (float)_samples;
        if (score > bestScore)
        {
          println("new highscore, Score: " + score);
          bestScore = score;
          bestMove = moves.get(m);
          bestStone = stones.get(s);
        } else println("lower score, score was: " + score);
      }
    }
    _playBoard.SelectStone(bestStone);
    return bestMove;
  }
  
  private int PlayRandomGame(Board pCopy)
  {
    if(debugInfo) println("\n---------------------\nRandom play\n---------------------\n");
    //while loop to loop as many times until the game has reached the end
    while(!pCopy.Finished())
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
              if(!madeMove) moves.remove(randomMoveId);
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
            
            if(madeMove) break;
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
    return pCopy.CheckWinner();
  }

  //be sure to pass in a copy of the board
  private int PlayRandomGameOld(Board pCopy)
  {
    while (true)
    {
      //make a random move
      //same code as the random agent
      IntList stoneIds = new IntList();
      ArrayList<Stone> stones = pCopy.GetStones(pCopy.GetActivePlayer());
      for (int i = 0; i < stones.size(); ++i)
      {
        stoneIds.append(i);
      }

      println("stone ids " + stoneIds.size());

      PVector move;


      while (true)
      {
        int random = (int)random(stoneIds.size());
        println("random stone id: " + random + " out of " + stoneIds.size());
        if (stoneIds.size() == 0)
        {
          //no stones are left to make a move
          return pCopy.CheckWinner();
        }
        int randomStoneId = stoneIds.get(random);
        if(debugInfo) println("got random stone id");
        Stone stone = stones.get(randomStoneId);
        pCopy.SelectStone(stone);

        ArrayList<PVector> moves = pCopy.GetMovesFor(stone);
        if (moves.size() == 0)
        {
          stoneIds.remove(random);
          break;
        } 
        else
        {
          ArrayList<PVector> jumps = pCopy.AvailableJumpsForStone(stone);
          if (jumps.size() == 0)
          {
            while(true)
            {
              if(moves.size() == 0)
              {
                stoneIds.remove(random);
                break;
              }
              int moveId = (int)random(moves.size());
              if(debugInfo) println("moveid " + moveId);
              move = moves.get(moveId);
              if(debugInfo) println("making move from " + stone.GetTile() + " to " + move);
              boolean madeMove = pCopy.MakeMove(move);
              if (madeMove) break;
              else
              {
                if(debugInfo) println("could not make move");
                moves.remove(moveId);
                continue;
              }
            }
          } 
          else
          {
            while(true)
            {
              if(moves.size() == 0)
              {
                stoneIds.remove(random);
                break;
              }
              random = (int)random(jumps.size());
              move = jumps.get(random);
              if(debugInfo) println("making move from " + stone.GetTile() + " to " + move);
              boolean madeMove = pCopy.MakeMove(move);
              if (madeMove) break;
              else
              {
                if(debugInfo) println("could not make move");
                continue;
              }
            }
          }
        }
      }

      //now check if there are still available moves
      //if there are continue the loop
      //if there aren't return the winner
      if (pCopy.Finished()) return pCopy.CheckWinner();
    }
  }
}
