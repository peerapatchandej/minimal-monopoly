using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Monopoly : MonoBehaviour
{
  [SerializeField]
  private PlayerController[] playerCtrls = default; //prototype ของจริงจะเป็น array จัดเรียง index ตามลำดับผู้เล่น

  [SerializeField]
  private BoardController boardCtrl = default;

  private int playerTurn;
  private int diceResult;
  private bool moveNextComplete = false;

  private IEnumerator Start()
  {
    playerCtrls[0].Setup(0);
    playerCtrls[1].Setup(5);

    yield return new WaitForSeconds(1f);

    playerTurn = 0;
    diceResult = 1;

    while (diceResult > playerTurn)
    {
      MoveNextSlot(() =>
      {
        moveNextComplete = true;
        diceResult--;
      });

      yield return new WaitUntil(() => moveNextComplete == true);

      moveNextComplete = false;
    }

    //=======================================================================

    playerTurn = 1;
    diceResult = 16;

    while (diceResult > 0)
    {
      MoveNextSlot(() =>
      {
        moveNextComplete = true;
        diceResult--;
      });

      yield return new WaitUntil(() => moveNextComplete == true);

      moveNextComplete = false;
    }
  }

  private void MoveNextSlot(Action onComplete = null)
  {
    //Clear old owned slot
    int previousIndex = playerCtrls[playerTurn].currentIndex - 1;

    BoardSlot boardSlot = boardCtrl.GetBoardSlot(previousIndex >= 0 ? previousIndex : 0);
    boardSlot.ClearOwnedSlot(playerTurn);

    int nextIndex = playerCtrls[playerTurn].currentIndex + 1;

    //ถ้าเดินจนวนสนามแล้วจะรีเซ็ต index
    if (nextIndex >= boardCtrl.GetBoardSlotCount())
    {
      nextIndex = 0;
    }

    //ก่อนทีจะ move ต้องเช็ค player ในช่องก่อนหน้าก่อนว่ามีมั้ย ถ้ามีก็ให้ player ในช่องนั้น swap in slot
    boardSlot = boardCtrl.GetBoardSlot(nextIndex);
    boardSlot.MoveToSlot(playerTurn, (position) =>
    {
      playerCtrls[playerTurn].Move(nextIndex, position, () =>
      {
        onComplete?.Invoke();
        if (diceResult == 0)
        {
          Debug.Log($"currentIndex : {playerCtrls[playerTurn].currentIndex}");
        }
      });
    }, (otherPlayer) =>
    {
      Debug.Log("otherPlayer " + otherPlayer);
      MoveSwapInSlot(otherPlayer, () =>
      {
        MoveNextSlot(onComplete);
      });
    });
  }

  private void MoveSwapInSlot(int index, Action onComplete = null)
  {
    BoardSlot boardSlot = boardCtrl.GetBoardSlot(playerCtrls[index].currentIndex);
    boardSlot.MoveToInnerSlot(index, (position) =>
    {
      boardSlot.ClearOwnedSlot(index);
      playerCtrls[index].Move(playerCtrls[index].currentIndex, position, () =>
      {
        onComplete?.Invoke();
      });
    });
  }
}
