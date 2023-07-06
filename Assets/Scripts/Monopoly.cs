using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Monopoly : MonoBehaviour
{
  [SerializeField]
  private PlayerController[] playerCtrls = default; //prototype ของจริงจะเป็น array

  [SerializeField]
  private BoardController boardCtrl = default;

  private int diceResult;
  private bool moveNextComplete = false;

  private IEnumerator Start()
  {
    playerCtrls[0].Setup(0);
    playerCtrls[1].Setup(5);

    yield return new WaitForSeconds(1f);

    diceResult = 1;

    //Clear old owned slot
    BoardSlot boardSlot = boardCtrl.GetBoardSlot(playerCtrls[0].originIndex);
    boardSlot.SetOwnedSlot(playerCtrls[0].ownedSlot, false);

    while (diceResult > 0)
    {
      MoveNextSlot(playerCtrls[0], () =>
      {
        moveNextComplete = true;
        diceResult--;
      });

      yield return new WaitUntil(() => moveNextComplete == true);

      moveNextComplete = false;
    }

    //=======================================================================

    diceResult = 16;

    //Clear old owned slot
    boardSlot = boardCtrl.GetBoardSlot(playerCtrls[1].originIndex);
    boardSlot.SetOwnedSlot(playerCtrls[1].ownedSlot, false);

    while (diceResult > 0)
    {
      MoveNextSlot(playerCtrls[1], () =>
      {
        moveNextComplete = true;
        diceResult--;
      });

      yield return new WaitUntil(() => moveNextComplete == true);

      moveNextComplete = false;
    }
  }

  private void MoveNextSlot(PlayerController playerCtrl, Action onComplete = null)
  {
    //ก่อนทีจะ move ต้องเช็ค player ในช่องก่อนหน้าก่อนว่ามีมั้ย ถ้ามีก็ให้ player ในช่องนั้น swap in slot

    int nextIndex = playerCtrl.currentIndex + 1;

    if (nextIndex >= boardCtrl.GetBoardSlotCount())
    {
      nextIndex = 0;
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

  private void MoveSwapInSlot(PlayerController playerCtrl, Action onComplete = null)
  {
    BoardSlot boardSlot = boardCtrl.GetBoardSlot(playerCtrl.currentIndex);
    BoardSlot.SlotData slotData = boardSlot.GetSlotData();
    playerCtrl.Move(playerCtrl.currentIndex, slotData.Position, onComplete);
  }
}
