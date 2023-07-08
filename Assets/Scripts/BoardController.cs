using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardController : MonoBehaviour
{
  [SerializeField]
  private GameObject redCorner = default;

  [SerializeField]
  private GameObject blueCorner = default;

  [SerializeField]
  private GameObject yellowCorner = default;

  [SerializeField]
  private GameObject greenCorner = default;

  [SerializeField]
  private GameObject topEdge = default;

  [SerializeField]
  private GameObject bottomEdge = default;

  [SerializeField]
  private GameObject leftEdge = default;

  [SerializeField]
  private GameObject rightEdge = default;

  [SerializeField]
  private Transform topParent = default;

  [SerializeField]
  private Transform bottomParent = default;

  [SerializeField]
  private Transform leftParent = default;

  [SerializeField]
  private Transform rightParent = default;

  [SerializeField]
  private RectTransform board = default;

  [SerializeField]
  private RectTransform centerArea = default;

  [SerializeField]
  private Transform pawnParent = default;

  [SerializeField]
  private GameObject[] pawns = default;

  [SerializeField]
  private List<BoardSlot> boardSlots = new List<BoardSlot>();

  public void CreateBoard(int maxEdge, int maxUpgradeSlot, List<int> playerSlotIndexes)
  {
    RectTransform edgeRect = topEdge.GetComponent<RectTransform>();
    RectTransform pawnRect = redCorner.GetComponent<RectTransform>();
    VerticalLayoutGroup boardVerticalLayout = board.GetComponent<VerticalLayoutGroup>();
    HorizontalLayoutGroup horizontalLayout = topParent.GetComponent<HorizontalLayoutGroup>();
    VerticalLayoutGroup verticalLayout = leftParent.GetComponent<VerticalLayoutGroup>();

    float defaultBoardHeight = (edgeRect.rect.height * Const.DEFAULT_EDGE) + (verticalLayout.spacing * (Const.DEFAULT_EDGE - 1)) + (pawnRect.rect.height * 2) + (boardVerticalLayout.spacing * 2);

    GameObject redCornerObj = Instantiate(redCorner, topParent);
    boardSlots.Add(redCornerObj.GetComponent<BoardSlot>());

    CreateEdge(topEdge, topParent, maxEdge, maxUpgradeSlot);

    GameObject blueCornerObj = Instantiate(blueCorner, topParent);
    boardSlots.Add(blueCornerObj.GetComponent<BoardSlot>());

    CreateEdge(rightEdge, rightParent, maxEdge, maxUpgradeSlot);

    GameObject yellowCornerObj = Instantiate(yellowCorner, bottomParent);
    boardSlots.Add(yellowCornerObj.GetComponent<BoardSlot>());

    CreateEdge(bottomEdge, bottomParent, maxEdge, maxUpgradeSlot);

    GameObject greenCornerObj = Instantiate(greenCorner, bottomParent);
    boardSlots.Add(greenCornerObj.GetComponent<BoardSlot>());

    CreateEdge(leftEdge, leftParent, maxEdge, maxUpgradeSlot);

    if (maxEdge != Const.DEFAULT_EDGE)
    {
      centerArea.sizeDelta = new Vector2((edgeRect.rect.width + horizontalLayout.spacing) * maxEdge - 18f, (edgeRect.rect.height + verticalLayout.spacing) * maxEdge - 18f);

      if (maxEdge > Const.DEFAULT_EDGE)
      {
        float currentBoardHeight = (edgeRect.rect.height * maxEdge) + (verticalLayout.spacing * (maxEdge - 1)) + (pawnRect.rect.height * 2) + (boardVerticalLayout.spacing * 2);
        board.localScale = new Vector2(defaultBoardHeight / currentBoardHeight, defaultBoardHeight / currentBoardHeight);
      }
    }

    LayoutRebuilder.ForceRebuildLayoutImmediate(board);

    foreach (var index in playerSlotIndexes)
    {
      GameObject pawn = null;

      switch ((PlayerType)index)
      {
        case PlayerType.Red:
          pawn = CreatePawn(pawns[index], redCornerObj.transform);
          pawn.transform.localPosition = new Vector2(pawn.transform.localPosition.x, -pawn.transform.localPosition.x);
          break;
        case PlayerType.Blue:
          pawn = CreatePawn(pawns[index], blueCornerObj.transform);
          pawn.transform.localPosition = new Vector2(pawn.transform.localPosition.x, pawn.transform.localPosition.x);
          break;
        case PlayerType.Yellow:
          pawn = CreatePawn(pawns[index], yellowCornerObj.transform);
          pawn.transform.localPosition = new Vector2(pawn.transform.localPosition.x, -pawn.transform.localPosition.x);
          break;
        case PlayerType.Green:
          pawn = CreatePawn(pawns[index], greenCornerObj.transform);
          pawn.transform.localPosition = new Vector2(pawn.transform.localPosition.x, pawn.transform.localPosition.x);
          break;
      }
    }
  }

  private void CreateEdge(GameObject obj, Transform parent, int maxEdge, int maxUpgradeSlot)
  {
    for (int i = 0; i < maxEdge; i++)
    {
      GameObject edge = Instantiate(obj, parent);
      BoardSlot slot = edge.GetComponent<BoardSlot>();
      if (slot)
      {
        slot.SetupUpdateSlot(maxUpgradeSlot);
      }

      boardSlots.Add(slot);
    }
  }

  private GameObject CreatePawn(GameObject obj, Transform parent)
  {
    GameObject pawn = Instantiate(obj, parent);
    pawn.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
    pawn.transform.SetParent(pawnParent);
    pawn.transform.localEulerAngles = Vector3.zero;
    return pawn;
  }

  public BoardSlot GetBoardSlot(int index)
  {
    return boardSlots[index];
  }

  public int GetBoardSlotCount()
  {
    return boardSlots.Count;
  }
}
