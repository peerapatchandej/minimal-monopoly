using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSlot : MonoBehaviour
{
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

  [SerializeField]
  private Transform centerSlot = default;

  [SerializeField]
  private Transform[] slots = default;

  private bool ownedCenterSlot = false;
  private bool[] ownedSlot = new bool[] { false, false, false, false };

  public SlotData GetSlotData()
  {
    if (!ownedCenterSlot && !ownedSlot[0])
    {
      //ownedCenterSlot = true;
      return new SlotData { Position = centerSlot.position, SlotType = SlotType.Center };
    }
    else
    {
      for (int i = 0; i < ownedSlot.Length; i++)
      {
        if (!ownedSlot[i])
        {
          //ownedSlot[i] = true;
          return new SlotData { Position = slots[i].position, SlotType = (SlotType)i };
        }
      }
    }

    return default;
  }

  public void SetOwnedSlot(int type, bool value)
  {
    if ((SlotType)type == SlotType.Center) ownedCenterSlot = value;
    else ownedSlot[type] = value;
  }
}
