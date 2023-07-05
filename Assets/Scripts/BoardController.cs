using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoardController : MonoBehaviour
{
  [SerializeField]
  private BoardSlot[] boardSlots = default;

  public BoardSlot GetBoardSlot(int index)
  {
    return boardSlots[index];
  }
}
