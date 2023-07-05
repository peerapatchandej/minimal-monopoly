using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSlot : MonoBehaviour
{
  [SerializeField]
  private Transform centerSlot = default;

  [SerializeField]
  private Transform[] slots = default;

  private bool ownedCenterSlot = false;
  private bool[] ownedSlot = new bool[] { false, false, false, false };

  public Vector2 GetSlotPosition()
  {
    if (!ownedCenterSlot && !ownedSlot[0])
    {
      ownedCenterSlot = true;
      return centerSlot.position;
    }
    else
    {
      for (int i = 0; i < ownedSlot.Length; i++)
      {
        if (!ownedSlot[i])
        {
          ownedSlot[i] = true;
          return slots[i].position;
        }
      }
    }

    return default;
  }
}
