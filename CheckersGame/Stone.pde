class Stone
{
  private Board _board;
  private PVector _tile;
  private color _color;
  private color _stroke;
  
  public Stone(Board parentBoard, PVector pTile, color pColor, color pStroke)
  { 
    _board = parentBoard;
    _color = pColor;
    _tile = pTile;
    _stroke = pStroke;
  }
  
  public void setTile(PVector pTile)
  {
    _tile = pTile;
  }
  
  public boolean IsClicked()
  {
    if(mousePressed)
    {
      PVector pos = getPosition();
      float distance = sqrt(pow(pos.x-mouseX,2)-pow(pos.y-mouseY,2));
      if(distance <= 20) return true;
    }
    return false;
  }
  
  public void Render()
  {
    ellipseMode(CORNER);
    fill(_color);
    stroke(_stroke);
    ellipse(getPosition().x, getPosition().y , 40, 40);
  }
  
  private PVector getPosition()
  {
    return new PVector(50 + 5 + _tile.x * 50, 50 + 5 + _tile.y * 50);
  }
  
}
