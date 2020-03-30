class Stone
{
  private Board _board;
  private PVector _tile;
  private color _color;
  private color _stroke;
  private boolean _selected = false;
  
  public Stone(Board parentBoard, PVector pTile, color pColor, color pStroke)
  { 
    _board = parentBoard;
    _color = pColor;
    _tile = pTile;
    _stroke = pStroke;
  }
  
  public color GetColor()
  {
    return _color;
  }
  
  public void SetTile(PVector pTile)
  {
    _tile = pTile;
  }
  
  public PVector GetTile()
  {
    return _tile;
  }
  
  public boolean IsClicked()
  {
    if(mousePressed)
    {
      PVector pos = getPosition();
      float distance = sqrt(pow(pos.x-mouseX,2)+pow(pos.y-mouseY,2));
      if(distance <= 20) return true;
    }
    return false;
  }
  
  public void Selected(boolean pSelected)
  {
    _selected = pSelected;
  }
  
  public void Render()
  {
    ellipseMode(CENTER);
    fill(_color);
    if(!_selected)
    {
      stroke(_stroke);
      strokeWeight(1);
    }
    else
    {
      stroke(70, 140, 255);
      strokeWeight(4);
    }
    ellipse(getPosition().x, getPosition().y , 40, 40);
  }
  
  private PVector getPosition()
  {
    return new PVector(50 + 25 + _tile.x * 50, 50 + 25 + _tile.y * 50);
  }
  
}
