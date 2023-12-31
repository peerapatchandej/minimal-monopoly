﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;

public class Monopoly : MonoBehaviour
{
  public struct State
  {
    public ResourceLoader ResourceLoader;
    public List<SelectedPlayerData> PlayerSlotIndexes;
    public int PlayerHealth;
    public int MaxEdge;
    public int MaxUpgradeSlot;
    public int DiceType;
    public int MaxDice;
    public Action OnLoadSceneMainMenu;
  }

  [SerializeField]
  private BoardController boardCtrl = default;

  [SerializeField]
  private BoardAction boardAction = default;

  [SerializeField]
  private Transform pawnParent = default;

  [SerializeField]
  private Button back = default;

  [SerializeField]
  private List<PlayerController> playerCtrls = new List<PlayerController>();

  private static State state;
  private static Action onSceneLoaded = null;

  private int currentTurn = 0;
  private int playerTurnIndex = -1;
  private int diceResult = 0;

  private bool initPlayer = false;
  private bool rollComplete = false;
  private bool moveNextComplete = false;

  private List<PlayerColor> scoreList = new List<PlayerColor>();

  public static void LoadScene(State state)
  {
    UnityAction<Scene, LoadSceneMode> onLoaded = null;
    onLoaded = (arg, mode) =>
    {
      OnSceneLoaded(state);
      SceneManager.sceneLoaded -= onLoaded;
    };

    SceneManager.sceneLoaded += onLoaded;
    SceneManager.LoadScene("Game");
  }

  private static void OnSceneLoaded(State stateParam)
  {
    state = stateParam;
    onSceneLoaded?.Invoke();
  }

  private void Awake()
  {
    onSceneLoaded = () =>
    {
      boardCtrl.CreateBoard(state.MaxEdge, state.MaxUpgradeSlot, state.DiceType, state.MaxDice, state.PlayerHealth, state.PlayerSlotIndexes);
    };

    back.onClick.AddListener(() =>
    {
      state.OnLoadSceneMainMenu?.Invoke();
      back.interactable = false;
    });
  }

  private IEnumerator Start()
  {
    yield return new WaitForEndOfFrame();

    FxStartGame fxStartGame = null;
    state.ResourceLoader.LoadAndCreateUI("Fx/FxStartGame", (obj) =>
    {
      fxStartGame = obj.GetComponent<FxStartGame>();
      fxStartGame.Setup();
    });

    yield return new WaitForSeconds(1f);

    Destroy(fxStartGame.gameObject);

    FxFindFirstPlayer fxFindFirstPlayer = null;
    state.ResourceLoader.LoadAndCreateUI("Fx/FxFindFirstPlayer", (obj) =>
    {
      fxFindFirstPlayer = obj.GetComponent<FxFindFirstPlayer>();
      fxFindFirstPlayer.Setup(() =>
      {
        StartCoroutine(SetupPlayer(() =>
        {
          Destroy(fxFindFirstPlayer.gameObject);
        }));
      });
    });

    yield return new WaitUntil(() => initPlayer);

    while (true)
    {
      while (playerCtrls[currentTurn].PlayerLose())
      {
        currentTurn++;
        if (currentTurn >= state.PlayerSlotIndexes.Count) currentTurn = 0;
        yield return new WaitForEndOfFrame();
      }

      if (scoreList.Count == state.PlayerSlotIndexes.Count - 1)
      {
        scoreList.Add(playerCtrls[currentTurn].playerColor);
        scoreList.Reverse();
        break;
      }

      state.ResourceLoader.LoadAndCreateUI("Fx/FxTurnBegin", (obj) =>
      {
        FxTurnBegin fxTurnBegin = obj.GetComponent<FxTurnBegin>();
        Color color = default;
        string turn = "";

        switch (playerCtrls[currentTurn].playerColor)
        {
          case PlayerColor.Red:
            color = Const.RED_COLOR;
            turn = "Red Player Turn";
            break;
          case PlayerColor.Blue:
            color = Const.BLUE_COLOR;
            turn = "Blue Player Turn";
            break;
          case PlayerColor.Yellow:
            color = Const.YELLOW_COLOR;
            turn = "Yellow Player Turn";
            break;
          case PlayerColor.Green:
            color = Const.GREEN_COLOR;
            turn = "Green Player Turn";
            break;
        }

        fxTurnBegin.Setup(color, turn);
      });

      rollComplete = false;
      boardCtrl.SetBorderColor((int)playerCtrls[currentTurn].playerColor);

      if (playerCtrls[currentTurn].playerType == PlayerType.Player)
      {
        RollDice((result) =>
        {
          diceResult = result;
          rollComplete = true;
        });
      }
      else if (playerCtrls[currentTurn].playerType == PlayerType.AI)
      {
        RollDiceAI((result) =>
        {
          diceResult = result;
          rollComplete = true;
        });
      }

      yield return new WaitUntil(() => rollComplete);

      while (diceResult > 0)
      {
        MoveNextSlot(() =>
        {
          moveNextComplete = true;
        });

        yield return new WaitUntil(() => moveNextComplete);

        moveNextComplete = false;
      }

      currentTurn++;
      if (currentTurn >= state.PlayerSlotIndexes.Count) currentTurn = 0;
    }

    state.ResourceLoader.LoadAndCreateUI("ScoreDialog", (obj) =>
    {
      UIScoreDialog dialog = obj.GetComponent<UIScoreDialog>();
      dialog.Setup(scoreList, () =>
      {
        LoadScene(state);
      }, state.OnLoadSceneMainMenu);
    });
  }

  private IEnumerator SetupPlayer(Action callback)
  {
    List<PlayerController> playerCtrlsTemp = new List<PlayerController>();

    for (int x = 0; x < pawnParent.childCount; x++)
    {
      PlayerController playerCtrl = pawnParent.GetChild(x).GetComponent<PlayerController>();
      playerCtrlsTemp.Add(playerCtrl);
    }

    int rollCount = 0;
    bool complete = false;

    while (rollCount < state.PlayerSlotIndexes.Count)
    {
      boardCtrl.SetBorderColor(state.PlayerSlotIndexes[rollCount].Index);
      complete = false;

      Action<int> onResult = (result) =>
      {
        if (diceResult < result)
        {
          diceResult = result;
          playerTurnIndex = rollCount;
        }

        rollCount++;
        complete = true;
      };

      if (state.PlayerSlotIndexes[rollCount].Type == PlayerType.Player)
      {
        RollDice((result) =>
        {
          onResult.Invoke(result);
        });
      }
      else if (state.PlayerSlotIndexes[rollCount].Type == PlayerType.AI)
      {
        RollDiceAI((result) =>
        {
          onResult.Invoke(result);
        });
      }

      yield return new WaitUntil(() => complete);
      yield return new WaitForSeconds(1f);
    }

    int sortCount = 0;
    int i = playerTurnIndex;

    while (sortCount < state.PlayerSlotIndexes.Count)
    {
      if (i < state.PlayerSlotIndexes.Count)
      {
        playerCtrls.Add(playerCtrlsTemp[i]);
        boardCtrl.GetBoardSlot(playerCtrls[playerCtrls.Count - 1].currentIndex).SetOwnedSlot(OwnedSlotType.Center, playerCtrls.Count - 1);
        i++;
        sortCount++;
      }
      else
      {
        i = 0;
      }
    }

    playerCtrlsTemp.Clear();
    callback?.Invoke();
    initPlayer = true;
  }

  private void RollDice(Action<int> callback)
  {
    if (boardAction) boardAction.EnableDicePage(callback);
  }

  private void RollDiceAI(Action<int> callback)
  {
    if (boardAction) boardAction.EnableDicePageAI(callback);
  }

  private void MoveNextSlot(Action onComplete = null)
  {
    PlayerController playerCtrl = playerCtrls[currentTurn];

    int previousIndex = playerCtrl.currentIndex - 1;
    BoardSlot boardSlot = boardCtrl.GetBoardSlot(previousIndex >= 0 ? previousIndex : boardCtrl.GetBoardSlotCount() - 1);
    boardSlot.ClearOwnedSlot(currentTurn);

    int nextIndex = playerCtrl.currentIndex + 1;

    if (nextIndex >= boardCtrl.GetBoardSlotCount())
    {
      nextIndex = 0;
    }

    boardSlot = boardCtrl.GetBoardSlot(nextIndex);
    boardSlot.MoveToSlot(currentTurn, (position) =>
    {
      playerCtrl.Move(nextIndex, position, () =>
      {
        boardSlot.ChangeColor(playerCtrl.playerColor);

        diceResult--;

        if (diceResult == 0)
        {
          previousIndex = playerCtrl.currentIndex - 1;
          boardSlot = boardCtrl.GetBoardSlot(previousIndex >= 0 ? previousIndex : boardCtrl.GetBoardSlotCount() - 1);
          boardSlot.ClearOwnedSlot(currentTurn);

          boardSlot = boardCtrl.GetBoardSlot(playerCtrl.currentIndex);
          if (boardSlot.GetBoardType() == BoardType.Corner)
          {
            if (playerCtrl.conerIndex == playerCtrl.currentIndex)
            {
              UpdateHealth(playerCtrl, Const.MAX_HEAL);
            }
            else
            {
              UpdateHealth(playerCtrl, Const.NORMAL_HEAL);
            }

            onComplete?.Invoke();
          }
          else if (boardSlot.GetBoardType() == BoardType.Edge)
          {
            Action onBuyArea = () =>
            {
              if (playerCtrl.playerType == PlayerType.Player)
              {
                boardAction.EnableBuyArea(() =>
                {
                  playerCtrl.UpgradeSlot();
                  boardSlot.UpgradeSlot((int)playerCtrl.playerColor);
                  UpdateHealth(playerCtrl, -Const.COST_BUY_AREA);
                }, () =>
                {
                  onComplete?.Invoke();
                });
              }
              else if (playerCtrl.playerType == PlayerType.AI)
              {
                if (playerCtrl.GetHealth() > 2 && boardCtrl.GetHighUpgradeWithExclude((int)playerCtrl.playerColor) < playerCtrl.GetHealth())
                {
                  playerCtrl.UpgradeSlot();
                  boardSlot.UpgradeSlot((int)playerCtrl.playerColor);
                  UpdateHealth(playerCtrl, -Const.COST_BUY_AREA);
                  onComplete?.Invoke();
                }
                else
                {
                  onComplete?.Invoke();
                }
              }
            };

            if (!boardSlot.SlotHasUpgrade())
            {
              onBuyArea.Invoke();
            }
            else
            {
              if (boardSlot.CheckOwner((int)playerCtrl.playerColor))
              {
                if (boardSlot.CanUpgradeArea())
                {
                  onBuyArea.Invoke();
                }
                else
                {
                  onComplete?.Invoke();
                }
              }
              else
              {
                UpdateHealth(playerCtrl, -boardSlot.GetUpgradeCount());
                playerCtrl.TakeDamage();
                boardSlot.ResetUpgradeSlot();
                onComplete?.Invoke();
              }
            }
          }
        }
        else
        {
          onComplete?.Invoke();
        }
      });
    }, (otherPlayer) =>
    {
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

  private void UpdateHealth(PlayerController playerCtrl, int value)
  {
    playerCtrl.UpdateHealth(value, () =>
    {
      BoardSlot boardSlot = boardCtrl.GetBoardSlot(playerCtrl.currentIndex);
      boardSlot.ClearOwnedSlot(currentTurn);
      boardCtrl.ClearAllBuyAreaWithPlayer((int)playerCtrl.playerColor);

      scoreList.Add(playerCtrl.playerColor);
    });

    boardCtrl.SetHealth((int)playerCtrl.playerColor, playerCtrl.GetHealth());
  }
}