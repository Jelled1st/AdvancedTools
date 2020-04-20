class Move
{
  public Move(Stone pStone, PVector pTo)
  {
    this(pStone, pTo, null);
  }
  
  public Move(Stone pStone, PVector pTo, Stone pClaimed)
  {
    stone = pStone;
    claimedStone = pClaimed;
    from = pStone.GetTile();
    to = pTo;
  }
  
  public final Stone stone;
  public final Stone claimedStone;
  public final PVector from;
  public final PVector to;
}
