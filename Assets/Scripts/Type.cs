using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoardType
{
  Corner,
  Edge
}

public enum OwnedSlotType
{
  One = 0,
  Two,
  Three,
  Four,
  Center
}

public enum CenterAreaPage
{
  Dice
}

public enum PlayerColor
{
  Red,
  Blue,
  Yellow,
  Green
}

public enum PlayerType
{
  Player,
  AI
}

public enum DiceType
{
  D4 = 4,
  D6 = 6,
  D8 = 8,
  D10 = 10,
  D12 = 12
}

public struct SelectedPlayerData
{
  public int Index;
  public PlayerType Type;
}