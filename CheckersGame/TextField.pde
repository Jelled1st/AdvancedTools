class TextField
{
  char[] _allowedKeys = {'0','1','2','3','4','5','6','7','8','9'};
  boolean[] _pressedKeys;
  
  boolean _pressedBackSpace = false;
  
  String _input = "";
  boolean _selected = false;
  
  PVector position;
  PVector size;
  String _text;

  public TextField(float x, float y, float sx, float sy, String pText)
  {
    this(new PVector(x, y), new PVector(sx, sy), pText);
  }

  public TextField(PVector pPosition, PVector pSize, String pText)
  {
    position = pPosition;
    size = pSize;
    _text = pText;
    
    _pressedKeys = new boolean[_allowedKeys.length];
    for(int i = 0; i < _pressedKeys.length; ++i)
    {
      _pressedKeys[i] = false;
    }
  }
  
  public void Update()
  {
    checkSelected();
    if(_selected)
    {
      if(keyPressed)
      {
        for(int i = 0; i < _allowedKeys.length; ++i)
        {
          if(key == _allowedKeys[i])
          {
            if(!_pressedKeys[i])   
            {
              _input += "" + _allowedKeys[i];
              _pressedKeys[i] = true;
            }
          }
        }
        if(key == BACKSPACE)
        {
          if(!_pressedBackSpace)
          {
            if(_input.length() >= 1) _input = _input.substring(0, _input.length()-1);
            _pressedBackSpace = true;
          }
        }
        else _pressedBackSpace = false;
      }
      else
      {
        for(int i = 0; i < _allowedKeys.length; ++i)
        {
          _pressedKeys[i] = false;
        }
        _pressedBackSpace = false;
      }
    }
  }
  
  public void Render()
  {
    stroke(0);
    rectMode(CENTER);
    textAlign(LEFT, CENTER);
    textSize(18);
    //button 1
    if(!_selected) fill(200, 200, 200);
    else fill(255,255,255);
    rect(position.x, position.y, size.x, size.y);
    fill(0);
    if(_input == "") text(_text, position.x - size.x/2+5, position.y);
    else text(_input, position.x - size.x/2, position.y);
  }
  
  public void SetInput(String input)
  {
    _input = input;
  }
  
  public String GetInput()
  {
    return _input;
  }
  
  public void SetSelected(boolean pSelect)
  {
    _selected = pSelect;
  }
  
  public boolean GetSelected()
  {
    return _selected;
  }
  
  private void checkSelected()
  {
    if (mouseX <= position.x+size.x/2 && mouseX >= position.x-size.x/2 && mouseY <= position.y+size.y/2 && mouseY >= position.y-size.y/2)
    {
      if(mousePressed) _selected = true;
    } 
    else if(mousePressed) _selected = false;
    
    if(_selected && keyPressed && key == ENTER) _selected = false;
  }
}
