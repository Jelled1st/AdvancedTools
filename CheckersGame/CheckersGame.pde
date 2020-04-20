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

TextField[] samplesField = new TextField[2];
TextField[] depthField = new TextField[2];

boolean testing = false;
  
void setup()
{
  size(500,580);
  
  _agents = new Agent[2];
  
  startButton = new Button(width / 2, 475, 100, 30, "Start!");
  
  selectAgentButtons[0] = new Button(75, 475, 100, 30, "Player");
  selectAgentButtons[1] = new Button(width-75, 475, 100, 30, "Player");
  
  samplesField[0] = new TextField(75, 515, 100, 30, "Samples");
  samplesField[1] = new TextField(width-75, 515, 100, 30, "Samples");
  
  depthField[0] = new TextField(75, 555, 100, 30, "Depth(-1)");
  depthField[1] = new TextField(width-75, 555, 100, 30, "Depth(-1)");
  
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
  
  if(testing)
  {
    selectAgentButtons[0].SetSelected(4);
    selectAgentButtons[1].SetSelected(5);
    
    _agents[0] = GetAgent(selectAgentButtons[0].GetSelected(), _board, -1);
    _agents[1] = GetAgent(selectAgentButtons[1].GetSelected(), _board, 1);
  }
}

void resetBoard()
{
  for(int i = 0; i < _agents.length; ++i)
  {
    _agents[i].Reset();
  }
  _board.Reset();
}

void resetGame()
{
  _board.Reset();
  for(int i = 0; i < _agents.length; ++i)
  {
    _agents[i].Reset();
  }
  
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
    if(selectAgentButtons[i].GetSelected() == AgentTypes.MONTECARLO || selectAgentButtons[i].GetSelected() == AgentTypes.ALPHA_AGENT)
    {
      samplesField[i].Update();
      samplesField[i].Render();
      
      depthField[i].Update();
      depthField[i].Render();
    }
    else if(selectAgentButtons[i].GetSelected() == AgentTypes.ALPHABETA)
    {
      depthField[i].Update();
      depthField[i].Render();
    }
  }
  if(state != GameState.GAME)
  {
    startButton.Render();
    if(startButton.Pressed())
    {
      for(int i = 0; i < selectAgentButtons.length; ++i)
      {
        if(selectAgentButtons[i].GetSelected() == AgentTypes.MONTECARLO)
        {
          int samples = 25;
          String in = samplesField[i].GetInput();
          if(in != "")
          {
            samples = Integer.valueOf(in);
          }
          int depth = -1;
          in = depthField[i].GetInput();
          if(in != "")
          {
            depth = Integer.valueOf(in);
          }
          _agents[i] = new MonteCarloAgent(_board, (i*2)-1, samples, depth);
        }
        else if(selectAgentButtons[i].GetSelected() == AgentTypes.ALPHABETA)
        {
          int depth = -1;
          String in = depthField[i].GetInput();
          if(in != "")
          {
            depth = Integer.valueOf(in);
          }
          _agents[i] = new AlphaBetaPruningAgent(_board, (i*2)-1, depth);
        }
        else if(selectAgentButtons[i].GetSelected() == AgentTypes.ALPHA_AGENT)
        {
          int samples = 25;
          String in = samplesField[i].GetInput();
          if(in != "")
          {
            samples = Integer.valueOf(in);
          }
          int depth = -1;
          in = depthField[i].GetInput();
          if(in != "")
          {
            depth = Integer.valueOf(in);
          }
          _agents[i] = new AlphaAgent(_board, (i*2)-1, depth, samples);
        }
      }
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
    textAlign(CENTER);
    text("Min Wins: " + wins[0], width / 2, 200);
    text("Max wins: " + wins[1], width / 2, 230);
    textSize(18);
    text("Total games played: " + gamesPlayed, width / 2, 400);
    textSize(26);
    text("Press space to continue!", width /2, 430 );
    if(keyPressed && key == ' ')
    {
      state = GameState.MENU;
      resetGame();
    }
  }
}

void updateGame()
{
  //if there is a winner or if the game is finished
  if(_board.Finished())
  {
    int winner = _board.CheckWinner();
    //println("\n----------------------\nWe have a result: " + winner + "\n----------------------\n");
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
  Board activeBoard = _board;
  activePlayer = _board.GetActivePlayer();
  int player = (int)(_board.GetActivePlayer()/2.0f+1.5f); //converts player to 1/2
  PVector move = null;
  move = _agents[player-1].MakeMove();
  if(activeBoard != _board || activePlayer != _board.GetActivePlayer())
  {
    //something changed while getting a move
    //abord
    return;
  }
  if(move != null && state == GameState.GAME)
  {
    _board.MakeMove(move);
    activePlayer = 0;
  }
}
