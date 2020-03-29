Board playBoard;

void setup()
{
  size(500,500);
  playBoard = new Board();
}

void draw()
{
  background(120);
  playBoard.Render();
}
