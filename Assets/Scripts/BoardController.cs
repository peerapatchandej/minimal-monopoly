using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
  private Transform diceParent = default;

  [SerializeField]
  private Image borderImage = default;

  [SerializeField]
  private GameObject[] pawns = default;

  [SerializeField]
  private GameObject[] dices = default;

  [SerializeField]
  private PlayerHealth playerHealth = default;

  [SerializeField]
  private List<BoardSlot> boardSlots = new List<BoardSlot>();

  public void CreateBoard(int maxEdge, int maxUpgradeSlot, int diceType, int maxDice, int playerHealth, List<int> playerSlotIndexes)
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

    this.playerHealth.Setup(playerSlotIndexes);

    foreach (var index in playerSlotIndexes)
    {
      GameObject pawn = null;
      PlayerController playerController = null;

      switch ((PlayerType)index)
      {
        case PlayerType.Red:
          pawn = CreatePawn(pawns[index], redCornerObj.transform);
          playerController = pawn.GetComponent<PlayerController>();
          playerController.Setup((PlayerType)index, playerHealth, 0);
          pawn.transform.localPosition = new Vector2(pawn.transform.localPosition.x, -pawn.transform.localPosition.x);
          break;
        case PlayerType.Blue:
          pawn = CreatePawn(pawns[index], blueCornerObj.transform);
          playerController = pawn.GetComponent<PlayerController>();
          playerController.Setup((PlayerType)index, playerHealth, maxEdge + 1);
          pawn.transform.localPosition = new Vector2(pawn.transform.localPosition.x, pawn.transform.localPosition.x);
          break;
        case PlayerType.Yellow:
          pawn = CreatePawn(pawns[index], yellowCornerObj.transform);
          playerController = pawn.GetComponent<PlayerController>();
          playerController.Setup((PlayerType)index, playerHealth, (maxEdge + 1) * 2);
          pawn.transform.localPosition = new Vector2(pawn.transform.localPosition.x, -pawn.transform.localPosition.x);
          break;
        case PlayerType.Green:
          pawn = CreatePawn(pawns[index], greenCornerObj.transform);
          playerController = pawn.GetComponent<PlayerController>();
          playerController.Setup((PlayerType)index, playerHealth, (maxEdge + 1) * 3);
          pawn.transform.localPosition = new Vector2(pawn.transform.localPosition.x, pawn.transform.localPosition.x);
          break;
      }

      this.playerHealth.SetHealth(index, playerHealth);
    }

    for (int i = 0; i < maxDice; i++)
    {
      switch ((DiceType)diceType)
      {
        case DiceType.D4:
          CreateDice(dices[0]);
          break;
        case DiceType.D6:
          CreateDice(dices[1]);
          break;
        case DiceType.D8:
          CreateDice(dices[2]);
          break;
        case DiceType.D10:
          CreateDice(dices[3]);
          break;
        case DiceType.D12:
          CreateDice(dices[4]);
          break;
      }
    }
  }

  public void SetBorderColor(int index)
  {
    switch ((PlayerType)index)
    {
      case PlayerType.Red:
        borderImage.color = Const.RED_COLOR;
        break;
      case PlayerType.Blue:
        borderImage.color = Const.BLUE_COLOR;
        break;
      case PlayerType.Yellow:
        borderImage.color = Const.YELLOW_COLOR;
        break;
      case PlayerType.Green:
        borderImage.color = Const.GREEN_COLOR;
        break;
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

  private void CreateDice(GameObject obj)
  {
    GameObject dice = Instantiate(obj, diceParent);
    dice.transform.SetAsFirstSibling();
  }

  public BoardSlot GetBoardSlot(int index)
  {
    return boardSlots[index];
  }

  public int GetBoardSlotCount()
  {
    return boardSlots.Count;
  }

  public void SetHealth(int index, int health)
  {
    playerHealth.SetHealth(index, health);
  }
}
