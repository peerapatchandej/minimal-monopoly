using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Monopoly : MonoBehaviour
{
  [SerializeField]
  private PlayerController playerCtrl = default; //prototype ของจริงจะเป็น array

  [SerializeField]
  private BoardController boardCtrl = default;

  private int diceResult;

  private IEnumerator Start()
  {
    playerCtrl.Setup(0);
    playerCtrl.originIndex = 0;
    playerCtrl.ownedSlot = 4; //center;

    yield return new WaitForSeconds(1f);

    diceResult = 21;
    bool complete = false;

    //Clear old owned slot
    BoardSlot boardSlot = boardCtrl.GetBoardSlot(playerCtrl.originIndex);
    boardSlot.SetOwnedSlot(playerCtrl.ownedSlot, false);

    while (diceResult > 0)
    {
      MoveNextSlot(() =>
      {
        complete = true;
        diceResult--;
      });

      yield return new WaitUntil(() => complete == true);

      complete = false;
    }
  }

  private void MoveNextSlot(Action onComplete = null)
  {
    int nextIndex = playerCtrl.currentIndex + 1;

    if (nextIndex >= boardCtrl.GetBoardSlotCount())
    {
      nextIndex = playerCtrl.originIndex;
    }

    BoardSlot boardSlot = boardCtrl.GetBoardSlot(nextIndex);
    BoardSlot.SlotData slotData = boardSlot.GetSlotData();
    playerCtrl.Move(nextIndex, slotData.Position, () =>
    {
      onComplete?.Invoke();
      if (diceResult == 0)
      {
        playerCtrl.originIndex = playerCtrl.currentIndex;
        playerCtrl.ownedSlot = (int)slotData.SlotType;

        BoardSlot boardSlot = boardCtrl.GetBoardSlot(playerCtrl.originIndex);
        boardSlot.SetOwnedSlot(playerCtrl.ownedSlot, true);

        Debug.Log($"originIndex : {playerCtrl.originIndex}");
        Debug.Log($"ownedSlot : {playerCtrl.ownedSlot}");
      }
    });
  }

  private void MoveSwapInSlot(Action onComplete = null)
  {
    BoardSlot boardSlot = boardCtrl.GetBoardSlot(playerCtrl.currentIndex);
    BoardSlot.SlotData slotData = boardSlot.GetSlotData();
    playerCtrl.Move(playerCtrl.currentIndex, slotData.Position, onComplete);
  }
}
