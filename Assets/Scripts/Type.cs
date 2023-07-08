using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SlotData
{
  public Vector2 Position;
  public SlotType SlotType;
}

public enum SlotType
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

public enum PlayerType
{
  Red,
  Blue,
  Yellow,
  Green
}

public enum DiceType
{
  D4 = 4,
  D6 = 6,
  D8 = 8,
  D10 = 10,
  D12 = 12
}