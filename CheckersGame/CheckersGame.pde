enum GameState
{
  MENU,
  GAME,
  WINNER,
}

Agent[] _agents;
Board _board;
GameState state = GameState.MENU; 
int activePlayer = 0;

int[] wins;

int gamesToPlay = 100;
int gamesPlayed;

boolean pressedButton = false;

Button selectAgentButtons[] = new Button[2];
Button startButton;
  
void setup()
{
  size(500,500);
  
  _agents = new Agent[2];
  
  startButton = new Button(width / 2, 475, 100, 30, "Start!");
  
  selectAgentButtons[0] = new Button(75, 475, 100, 30, "Player");
  selectAgentButtons[1] = new Button(width-75, 475, 100, 30, "Player");
  
  for(int i = 0; i < selectAgentButtons.length; ++i)
  {
    //skip zero in the agents, since that is the default selection
    for(int a = 1; a < AgentTypes.AMOUNT; ++a)
    {
      selectAgentButtons[i].AddOption(AgentTypes.ToString(a));
    }
  }
  
  _board = new Board();
  _agents[0] = new PlayerAgent(_board, -1);
  _agents[1] = new PlayerAgent(_board, 1);
  resetGame();
}

void resetBoard()
{
  _board.Reset();
}

void resetGame()
{
  _board.Reset();
  
  wins = new int[2];
  wins[0] = 0;
  wins[1] = 0;
  gamesPlayed  = 0;
}

void draw()
{
  background(255);
  if(state == GameState.GAME) updateGame();
  handleUI();
  if(keyPressed && (key == 'R' || key == 'r'))
  {
    resetGame();
  }
}

void handleUI()
{
  for(int i = 0; i < selectAgentButtons.length; ++i)
  {
    selectAgentButtons[i].Render();
    if(selectAgentButtons[i].Pressed())
    {
      _agents[i] = GetAgent(selectAgentButtons[i].GetSelected(), _board, i*2-1);
    }
  }
  if(state != GameState.GAME)
  {
    startButton.Render();
    if(startButton.Pressed())
    {
      state = GameState.GAME;
      resetGame();
    }
  }
  textSize(16);
  fill(0, 0, 0);
  textAlign(LEFT, TOP);
  text("Player min: " + _agents[0].Name(), 5, 5);
  text("wins: " + wins[0], 5, 25);
  textAlign(RIGHT, TOP);
  text("Player max: " + _agents[1].Name(), width-5, 5);
  text("wins: " + wins[1], width-5, 25);
  
  if(state == GameState.GAME)
  {
    fill(255, 0, 0);
    textAlign(CENTER, TOP);
    text("Turn of: " + _board.GetActivePlayer(), width / 2, 30);
    text("Played " + gamesPlayed + " games", width /2, 450);
  }
  if(state == GameState.WINNER)
  {
    textSize(24);
    fill(0, 0, 255);
    text("Min Wins: " + wins[0], width / 2, height / 4);
    text("Max wins: " + wins[1], width / 2, height / 4 + 30);
    textSize(18);
    text("Total games played: " + gamesPlayed, width / 2, height / 4 * 3);
    textSize(26);
    text("Press mouse to continue!", width /2, height / 4 * 3 + 30 );
    if(mousePressed)
    {
      state = GameState.MENU;
      resetGame();
    }
  }
}

void updateGame()
{
  int winner = _board.CheckWinner();
  //if there is a winner or if the game is finished
  if(winner != 0 || _board.Finished())
  {
    println("\n----------------------\nWe have a result: " + winner + "\n----------------------\n");
    handleWin(winner);
    return;
  }
  thread("makeMove");
  _board.Render();
}

void handleWin(int winner)
{
  if(winner != 0) ++wins[(int)(winner/2.0f+1.5f)-1];
  ++gamesPlayed;
  if(gamesPlayed >= gamesToPlay) state = GameState.WINNER;
  resetBoard();
}

void makeMove()
{
  if(_board.GetActivePlayer() == activePlayer) return;
  activePlayer = _board.GetActivePlayer();
  int player = (int)(_board.GetActivePlayer()/2.0f+1.5f); //converts player to 1/2
  PVector move = _agents[player-1].MakeMove();
  if(move != null)
  {
    _board.MakeMove(move);
    activePlayer = 0;
  }
}
