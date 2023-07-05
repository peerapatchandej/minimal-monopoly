using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Monopoly : MonoBehaviour
{
  [SerializeField]
  private PlayerController playerCtrl = default; //prototype ของจริงจะเป็น array

  [SerializeField]
  private BoardController boardCtrl = default;

  private IEnumerator Start()
  {
    yield return new WaitForSeconds(1f);

    MoveNextSlot();

    yield return new WaitForSeconds(2f);

    MoveSwapInSlot();
  }

  private void MoveNextSlot()
  {
    int nextIndex = playerCtrl.currentIndex + 1;
    BoardSlot boardSlot = boardCtrl.GetBoardSlot(nextIndex);
    playerCtrl.Move(nextIndex, boardSlot.GetSlotPosition());
  }

  private void MoveSwapInSlot()
  {
    BoardSlot boardSlot = boardCtrl.GetBoardSlot(playerCtrl.currentIndex);
    playerCtrl.Move(playerCtrl.currentIndex, boardSlot.GetSlotPosition());
  }
}
