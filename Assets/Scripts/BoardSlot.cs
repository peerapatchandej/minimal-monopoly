using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BoardSlot : MonoBehaviour
{
  [SerializeField]
  private Transform centerSlot = default;

  [SerializeField]
  private Transform[] slots = default;

  [SerializeField]
  private Image[] upgradeSlots = default;

  [SerializeField]  //temporary
  private int ownedCenterSlotIndex = -1;                         //Player's index owned center slot

  [SerializeField]  //temporary
  private int[] ownedSlotIndex = new int[] { -1, -1, -1, -1 };   //Player's index owned these slot

  private int playerUpgrade = -1; //Player's index
  private int upgradeCount = 0;

  public void MoveToSlot(int playerIndex, Action<Vector2> onMove, Action<int> onSwapInSlot = null)
  {
    if (ownedCenterSlotIndex == -1)
    {
      bool isEmpty = true;

      for (int i = 0; i < ownedSlotIndex.Length; i++)
      {
        if (ownedSlotIndex[i] != -1) isEmpty = false;
      }

      if (isEmpty)
      {
        SetOwnedSlot(SlotType.Center, playerIndex);
        onMove?.Invoke(centerSlot.position);
      }
      else
      {
        MoveToInnerSlot(playerIndex, onMove);
      }
    }
    else
    {
      onSwapInSlot?.Invoke(ownedCenterSlotIndex);
    }
  }

  public void MoveToInnerSlot(int playerIndex, Action<Vector2> onMove)
  {
    for (int i = 0; i < ownedSlotIndex.Length; i++)
    {
      if (ownedSlotIndex[i] == -1)
      {
        SetOwnedSlot((SlotType)i, playerIndex);
        onMove?.Invoke(slots[i].position);
        break;
      }
    }
  }

  public void ClearOwnedSlot(int playerIndex)
  {
    if (ownedCenterSlotIndex == playerIndex)
    {
      ownedCenterSlotIndex = -1;
    }
    else
    {
      int foundIndex = ownedSlotIndex.ToList().FindIndex(slot => slot == playerIndex);
      if (foundIndex != -1) ownedSlotIndex[foundIndex] = -1;
    }
  }

  public void UpgradeSlot(int playerIndex)
  {
    if (upgradeCount >= upgradeSlots.Length)
    {
      Debug.Log($"Player {playerIndex} exceed limit upgrade");
      return;
    }

    playerUpgrade = playerIndex;
    Color color = default;

    if (playerUpgrade == 0) color = Const.RED_COLOR;
    else if (playerUpgrade == 1) color = Const.BLUE_COLOR;
    else if (playerUpgrade == 2) color = Const.YELLOW_COLOR;
    else if (playerUpgrade == 3) color = Const.GREEN_COLOR;

    upgradeSlots[upgradeCount].color = color;
    upgradeCount++;
  }

  //for check take damage and owner
  public bool SlotHasUpgrade()
  {
    return playerUpgrade != -1;
  }

  public bool CheckOwner(int playerIndex)
  {
    return playerUpgrade == playerIndex;
  }

  public void ResetUpgradeSlot()
  {
    playerUpgrade = -1;
    upgradeCount = 0;

    foreach (var slot in upgradeSlots)
    {
      slot.color = Color.white;
    }
  }

  private void SetOwnedSlot(SlotType type, int playerIndex)
  {
    if (type == SlotType.Center) ownedCenterSlotIndex = playerIndex;
    else ownedSlotIndex[(int)type] = playerIndex;
  }
}
