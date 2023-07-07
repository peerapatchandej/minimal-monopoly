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