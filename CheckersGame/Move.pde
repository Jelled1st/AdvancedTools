class Move
{
  public Move(Stone pStone, PVector pTo)
  {
    stone = pStone;
    from = pStone.GetTile();
    to = pTo;
  }
  
  public final Stone stone;
  public final PVector from;
  public final PVector to;
}
