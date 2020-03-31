class Board
{
  color whiteColor = color(255,255,220);
  color blackColor = color(84,27,33);
  ArrayList<Stone> _blackStones;
  ArrayList<Stone> _whiteStones;
  int _activePlayer = -1;
  Stone _selectedStone = null;
  
  public Board()
  {
    _blackStones = new ArrayList<Stone>();
    _whiteStones = new ArrayList<Stone>();
    for(int i = 0; i < 12; ++i)
    {
      int x = (i/3);
      int y = (i%3);
      PVector blackStoneTile = new PVector(x*2, y+5);
      PVector whiteStoneTile = new PVector(x*2+1, y);
      if(y%2==1)
      {
        //if y is uneven
        blackStoneTile.x += 1;
        whiteStoneTile.x-=1;
      }
      Stone black = new Stone(this, blackStoneTile, blackColor, whiteColor);
      Stone white = new Stone(this, whiteStoneTile, whiteColor, blackColor);
      _blackStones.add(black);
      _whiteStones.add(white);
    }
  }
  
  private Board(Board pCopy)
  {
    whiteColor = pCopy.whiteColor;
    blackColor = pCopy.blackColor;
    _blackStones = new ArrayList<Stone>();
    _whiteStones = new ArrayList<Stone>();
    for(int i = 0; i < pCopy._whiteStones.size(); ++i)
    {
      Stone stone = pCopy._whiteStones.get(i);
      _whiteStones.add(new Stone(this, stone.GetTile(), stone.GetColor(), stone.GetStroke()));
    }
    for(int i = 0; i < pCopy._blackStones.size(); ++i)
    {
      Stone stone = pCopy._blackStones.get(i);
      _blackStones.add(new Stone(this, stone.GetTile(), stone.GetColor(), stone.GetStroke()));
    }
    _activePlayer = pCopy._activePlayer;
    if(pCopy._selectedStone == null) _selectedStone = null;
    else
    {
      if(pCopy._selectedStone.GetColor() == pCopy.whiteColor) _selectedStone = _whiteStones.get(pCopy.GetSelectedIndex());
      else _selectedStone = _blackStones.get(pCopy.GetSelectedIndex());
    }
  }
  
  public int GetSelectedIndex()
  {
    if(_selectedStone == null) return -1;
    if(_selectedStone.GetColor() == whiteColor)
    {
      for(int i = 0; i < _whiteStones.size(); ++i)
      {
        if(_selectedStone == _whiteStones.get(i)) return i;
      }
    }
    else
    {
      for(int i = 0; i < _blackStones.size(); ++i)
      {
        if(_selectedStone == _blackStones.get(i)) return i;
      }
    }
    return -1;
  }
  
  public Board Copy()
  {
    Board copy = new Board(this);
    return copy;
  }
    
  public void Render()
  {
    drawBoard(8, 8);
    for(int i = 0; i < _whiteStones.size(); ++i)
    {
      _whiteStones.get(i).Render(i);
    }
    for(int i = 0; i < _blackStones.size(); ++i)
    {
      _blackStones.get(i).Render(i);
    }
  }
  
  public ArrayList<Stone> GetStones(int stoneSet)
  {
    if(stoneSet == 0 || stoneSet == -1) return _whiteStones;
    else if(stoneSet == 1) return _blackStones;
    else return null;
  }
  
  public int GetActivePlayer()
  {
    return _activePlayer;
  }
  
  public boolean IsValidMove(PVector move)
  {
    if(_selectedStone == null) return false;
    boolean validMove = false;
    
    if(move.x < 0 || move.x >= 8 || move.y < 0 || move.y >= 8) return false;
    
    PVector relativeMove = new PVector(move.x-_selectedStone.GetTile().x, move.y-_selectedStone.GetTile().y);
    
    boolean jumpMove = false;
    
    //make a hit, get other stone
    if((relativeMove.x == -2 || relativeMove.x == 2) && (relativeMove.y == -2 || relativeMove.y == 2))
    {
      Stone stone = GetStoneAtTile(move);
      if(stone != null) validMove = false;
      PVector jumpOver = new PVector(move.x - relativeMove.x/2, move.y - relativeMove.y/2);
      stone = GetStoneAtTile(jumpOver); 
      if(stone != null)
      {
        if(_activePlayer == -1 && stone.GetColor() == blackColor)
        {
          validMove = true;
          jumpMove = true;
        }
        else if(_activePlayer == 1 && stone.GetColor() == whiteColor)
        {
          validMove = true;
          jumpMove = true;
        }
      }
    }
    
    //if the validMove has not yet been set to true
    //that means that the player missed an available jump
    //if there is any
    if(validMove == false && availableJump()) return false;
    
    //regular move
    //cannot go backwards
    if((relativeMove.x == -1 || relativeMove.x == 1) && (relativeMove.y == -1 || relativeMove.y == 1))
    {
      if(_activePlayer == -1 && relativeMove.y == -1) validMove = false;
      else if(_activePlayer == 1 && relativeMove.y == 1) validMove = false;
      else
      {
        //selected tile and stone are in diagonals of each other
        Stone stone = GetStoneAtTile(move);
        if(stone == null) validMove = true;
      }
    }
    
    return validMove;
  }
  
  public boolean MakeMove(PVector move)
  {
    if(_selectedStone == null) return false;
    boolean validMove = false;
    println("passed move: " + move);
    
    if(move.x < 0 || move.x >= 8 || move.y < 0 || move.y >= 8) return false;
    
    PVector relativeMove = new PVector(move.x-_selectedStone.GetTile().x, move.y-_selectedStone.GetTile().y);
    
    boolean jumpMove = false;
    
    //make a hit, get other stone
    if((relativeMove.x == -2 || relativeMove.x == 2) && (relativeMove.y == -2 || relativeMove.y == 2))
    {
      Stone stone = GetStoneAtTile(move);
      if(stone != null) validMove = false;
      PVector jumpOver = new PVector(move.x - relativeMove.x/2, move.y - relativeMove.y/2);
      stone = GetStoneAtTile(jumpOver); 
      if(stone != null)
      {
        if(_activePlayer == -1 && stone.GetColor() == blackColor)
        {
          validMove = true;
          _blackStones.remove(stone);
          jumpMove = true;
        }
        else if(_activePlayer == 1 && stone.GetColor() == whiteColor)
        {
          validMove = true;
          _whiteStones.remove(stone);
          jumpMove = true;
        }
      }
    }
    
    //if the validMove has not yet been set to true
    //that means that the player missed an available jump
    //if there is any
    if(validMove == false && availableJump()) return false;
    //if the move is already valid, skip this check
    println("valid move: " + validMove);
    if(!validMove)
    {
      //regular move
      //cannot go backwards
      if((relativeMove.x == -1 || relativeMove.x == 1) && (relativeMove.y == -1 || relativeMove.y == 1))
      {
        if(_activePlayer == -1 && relativeMove.y == -1) validMove = false;
        else if(_activePlayer == 1 && relativeMove.y == 1) validMove = false;
        else
        {
          //selected tile and stone are in diagonals of each other
          Stone stone = GetStoneAtTile(move);
          if(stone == null) validMove = true;
        }
      }
    }
    
    println("valid move: " + validMove);
    if(validMove)
    {
      _selectedStone.SetTile(move);
        println("\n\nMove allowed, moved selected stone to " + move);
      //if there is another jump available for the stone that just moved
      //do not switch player, because that player gets to jump again
      boolean anotherJump = AvailableJumpsForStone(_selectedStone).size() > 0;
      if(!jumpMove || !anotherJump)
      {
        println("Switched active player on board\n\n");
        _activePlayer = -_activePlayer;
        _selectedStone.Selected(false);
        _selectedStone = null;
      }
    }
    return validMove;
  }
  
  public int CheckWinner()
  {
    if(_blackStones.size() == 0) return -1;
    if(_whiteStones.size() == 0) return 1;
    return 0;
  }
  
  //return if there are still available moves
  public boolean Finished()
  {
    if(_whiteStones.size() == 0 || _blackStones.size() == 0) return true;
    for(int w = 0; w < _whiteStones.size(); ++w)
    {
      for(int b = 0; b < _blackStones.size(); ++b)
      {
        PVector whiteTile = _whiteStones.get(w).GetTile();
        PVector blackTile = _blackStones.get(b).GetTile();
        PVector tileDifference = new PVector(whiteTile.x - blackTile.x, whiteTile.y - blackTile.x);
        if(tileDifference.y > 0) return false;
        if(tileDifference.y >= -1)
        {
          if(tileDifference.x >= -1 && tileDifference.x <= 1)
          {
            return false;
          }
        }
      }
    }
    return true;
  }
  
  public boolean SelectStone(Stone pStone)
  {
    for(int i = 0; i < _whiteStones.size(); ++i)
    {
      if(_whiteStones.get(i) == pStone)
      {
        if(_selectedStone != null) _selectedStone.Selected(false);
        _selectedStone = _whiteStones.get(i);
        _selectedStone.Selected(true);
        return true;
      }
    }
    for(int i = 0; i < _blackStones.size(); ++i)
    {
      if(_blackStones.get(i) == pStone)
      {
        if(_selectedStone != null) _selectedStone.Selected(false);
        _selectedStone = _blackStones.get(i);
        _selectedStone.Selected(true);
        return true;
      }
    }
    return false;
  }
  
  public boolean SelectStone(int index)
  {
    if(_activePlayer == -1)
    {
      if(index > 0 && index < _whiteStones.size())
      {
        _selectedStone.Selected(false);
        _selectedStone = _whiteStones.get(index);
        return true;
      }
    }
    else
    {
      if(index > 0 && index < _blackStones.size())
      {
        _selectedStone.Selected(false);
        _selectedStone = _blackStones.get(index);
        return true;
      }
    }
    return false;
  }
  
  public Stone GetSelectedStone()
  {
    return _selectedStone;
  }
  
  public PVector GetTileAtPosition(PVector pPosition)
  {
    pPosition.x -= 50;
    pPosition.y -= 50;
    PVector tile = new PVector((int)pPosition.x / 50, (int)pPosition.y / 50);
    if(tile.x < 0 || tile.y < 0 || tile.x >= 8 || tile.y >= 8)
    {
      return null;
    }
    return tile;
  }
  
  private Stone GetStoneAtTile(PVector pTile)
  {
    for(int i = 0; i < _whiteStones.size(); ++i)
    {
      PVector stoneTile = _whiteStones.get(i).GetTile();
      if(stoneTile.x == pTile.x && stoneTile.y == pTile.y) return _whiteStones.get(i);
    }
    for(int i = 0; i < _blackStones.size(); ++i)
    {
      PVector stoneTile = _blackStones.get(i).GetTile();
      if(stoneTile.x == pTile.x && stoneTile.y == pTile.y)
      {
        return _blackStones.get(i);
      }
    }
    return null;
  }
  
  public ArrayList<PVector> AvailableJumpsForStone(Stone stone)
  {
    ArrayList<PVector> jumps = null;
    if(stone == null) return jumps;
    jumps = new ArrayList<PVector>();
    PVector[] directions = { new PVector(1, 1), new PVector(-1, 1), new PVector(1, -1), new PVector(-1, -1) };
    if(_activePlayer == -1)
    {
        for(int d = 0; d < directions.length; ++d)
        {
          PVector tile = new PVector(stone.GetTile().x + directions[d].x, stone.GetTile().y + directions[d].y);
          Stone other = GetStoneAtTile(tile);
          if(other != null && other.GetColor() == blackColor)
          {
            PVector overTile = new PVector(tile.x + directions[d].x, tile.y + directions[d].y);
            if(overTile.x > 0 && overTile.x < 8 && overTile.y > 0 && overTile.y < 8)
            {
              if(GetStoneAtTile(overTile) == null)
              {
                //there is no stone behind the black stone
                jumps.add(overTile);
              }
            }
          }
        }
      return jumps;
    }
    else
    {
        for(int d = 0; d < directions.length; ++d)
        {
          PVector tile = new PVector(stone.GetTile().x + directions[d].x, stone.GetTile().y + directions[d].y);
          Stone other = GetStoneAtTile(tile);
          if(other != null && other.GetColor() == whiteColor)
          {
            PVector overTile = new PVector(tile.x + directions[d].x, tile.y + directions[d].y);
            if(overTile.x > 0 && overTile.x < 8 && overTile.y > 0 && overTile.y < 8)
            {
              if(GetStoneAtTile(overTile) == null)
              {
                //there is no stone behind the black stone
                jumps.add(overTile);
              }
            }
          }
        }
      return jumps;
    }
  }
  
  private boolean availableJump()
  {
    PVector[] directions = { new PVector(1, 1), new PVector(-1, 1), new PVector(1, -1), new PVector(-1, -1) };
    if(_activePlayer == -1)
    {
      for(int i = 0; i < _whiteStones.size(); ++i)
      {
        Stone stone = _whiteStones.get(i);
        for(int d = 0; d < directions.length; ++d)
        {
          PVector tile = new PVector(stone.GetTile().x + directions[d].x, stone.GetTile().y + directions[d].y);
          Stone other = GetStoneAtTile(tile);
          if(other != null && other.GetColor() == blackColor)
          {
            PVector overTile = new PVector(tile.x + directions[d].x, tile.y + directions[d].y);
            if(overTile.x > 0 && overTile.x < 8 && overTile.y > 0 && overTile.y < 8)
            {
              if(GetStoneAtTile(overTile) == null)
              {
                //there is no stone behind the black stone
                return true;
              }
            }
          }
        }
      }
      return false;
    }
    else
    {
      for(int i = 0; i < _blackStones.size(); ++i)
      {
        Stone stone = _blackStones.get(i);
        for(int d = 0; d < directions.length; ++d)
        {
          PVector tile = new PVector(stone.GetTile().x + directions[d].x, stone.GetTile().y + directions[d].y);
          Stone other = GetStoneAtTile(tile);
          if(other != null && other.GetColor() == whiteColor)
          {
            PVector overTile = new PVector(tile.x + directions[d].x, tile.y + directions[d].y);
            if(overTile.x > 0 && overTile.x < 8 && overTile.y > 0 && overTile.y < 8)
            {
              if(GetStoneAtTile(overTile) == null)
              {
                //there is no stone behind the black stone
                return true;
              }
            }
          }
        }
      }
      return false;
    }
  }
  
  public ArrayList<PVector> GetMovesFor(Stone stone)
  {
    ArrayList<PVector> moves = new ArrayList<PVector>();
    ArrayList<PVector> jumps = AvailableJumpsForStone(stone);
    for(int i = 0; i < jumps.size(); ++i)
    {
      moves.add(jumps.get(i));
    }
    PVector[] directions = { new PVector(1, 1), new PVector(-1, 1), new PVector(1, -1), new PVector(-1, -1) };
    //for white the player can only move down (except for jumping)
    int startD = 0;
    //for black the player can only move up (except for jumping)
    if(_activePlayer == 1) startD = 2;
    for(int d = startD; d < directions.length; ++d)
    {
      PVector tile = new PVector(stone.GetTile().x + directions[d].x, stone.GetTile().y + directions[d].y);
      if(GetStoneAtTile(tile) == null && tile.x >= 0 && tile.x < 8 && tile.y >= 0 && tile.y < 8)
      {
        moves.add(tile);
      }
    }
    
    return moves;
  }
  
  private color getActivePlayerColor()
  {
    if(_activePlayer == -1) return whiteColor;
    else return blackColor;
  }
  
  private color getOppositePlayerColor()
  {
    if(_activePlayer == -1) return blackColor;
    else return whiteColor;
  }
  
  private void drawBoard(int pX, int pY)
  {
    stroke(0,0,0);
    strokeWeight(1);
    for(int y = 0; y < pY; ++y)
    {
      for(int x = 0; x < pX; ++x)
      {
        if((y+x)%2 == 0) fill(whiteColor);
        else fill(blackColor);
        rect(50+x*50, 50+y*50, 50, 50);
      }
    }
  }
  
}
