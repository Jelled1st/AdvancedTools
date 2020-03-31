enum GameState
{
  MENU,
  GAME,
  WINNER,
}

Agent[] _agents;
Board _board;
int winner = 0;
GameState state = GameState.GAME; 
int activePlayer = 0;
  
void setup()
{
  size(500,500);
  
  _board = new Board();
  _agents = new Agent[2];
  _agents[0] = new RandomAgent(_board, -1);
  _agents[1] = new MonteCarloAgent(_board, 1);
  //_agents[1] = new PlayerAgent(_board, 1);
}

void draw()
{
  background(120);
  updateGame();
  showHUD();
  if(keyPressed && (key == 'R' || key == 'r'))
  {
    resetGame();
  }
}

void showHUD()
{
  textAlign(CENTER, CENTER);
  textSize(16);
  fill(0, 0, 255);
  text("Player min: " + _agents[0].Name(), 100, 20);
  text("Player max: " + _agents[1].Name(), width-100, 20);
  text("Turn of: " + _board.GetActivePlayer(), width / 2, 30);
  if(winner != 0)
  {
    textSize(24);
    fill(0, 0, 255);
    text("Winner: " + winner, width / 2, height / 4);
    if(mousePressed)
    {
      resetGame();
    }
  }
}

void updateGame()
{
  if(_board.GetStones(-1).size() == 0)
  {
    winner = 1;
    state = GameState.WINNER;
    return;
  }
  else if(_board.GetStones(1).size() == 0)
  {
    winner = -1;
    state = GameState.WINNER;
    return;
  }
  thread("makeMove");
  _board.Render();
}

void makeMove()
{
  if(_board.GetActivePlayer() == activePlayer) return;
  println("\n--------------------\nSwitched player\n--------------------\n");
  activePlayer = _board.GetActivePlayer();
  int player = (int)(_board.GetActivePlayer()/2.0f+1.5f); //converts player to 1/2
  PVector move = _agents[player-1].MakeMove();
  if(move != null)
  {
    _board.MakeMove(move);
    activePlayer = 0;
  }
}

void resetGame()
{
  _board = new Board();
  _agents[0] = new RandomAgent(_board, -1);
  _agents[1] = new MonteCarloAgent(_board, 1);
  winner = 0;
  state = GameState.GAME;
}
