class Button
{
  boolean continuesPress= false;
  boolean pressed= false;
  int selectedOption;
  ArrayList<String> options = new ArrayList<String>();

  PVector position;
  PVector size;

  public Button(float x, float y, float sx, float sy, String option)
  {
    position = new PVector(x, y);
    size = new PVector(sx, sy);
    options.add(option);
  }

  public Button(PVector pPosition, PVector pSize, String option)
  {
    position = pPosition;
    size = pSize;
    options.add(option);
  }

  public void AddOption(String option)
  {
    options.add(option);
  }

  public void SetContinues(boolean pBool)
  {
    continuesPress = pBool;
  }
  
  private void cycleOptions(int amount)
  {
    selectedOption += amount;
    if(selectedOption >= options.size()) selectedOption = 0;
    else if(selectedOption < 0) selectedOption = options.size() - 1;
  }
  
  public void SetSelected(int option)
  {
    selectedOption = option;
    cycleOptions(0);
  }
  
  public int GetSelected()
  {
    return selectedOption;
  }

  public void Render()
  {
    stroke(0);
    strokeWeight(1);
    rectMode(CENTER);
    textAlign(CENTER, CENTER);
    textSize(18);
    //button 1
    fill(200, 200, 200);
    rect(position.x, position.y, size.x, size.y);
    fill(0);
    text(options.get(selectedOption), position.x, position.y);
  }

  public boolean Pressed()
  {
    if (mouseX <= position.x+size.x/2 && mouseX >= position.x-size.x/2 && mouseY <= position.y+size.y/2 && mouseY >= position.y-size.y/2 && mousePressed)
    {
      if (continuesPress || !pressed)
      {
        pressed = true;
        if(mouseButton == LEFT) cycleOptions(1);
        else if(mouseButton == RIGHT) cycleOptions(-1);
      }
    } 
    else pressed = false;
    return pressed;
  }
}
