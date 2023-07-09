using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Resources;

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
    //Play Start Match Animation

    yield return new WaitForSeconds(1f);

    StartCoroutine(SetupPlayer());

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

    // show result dialog
    state.ResourceLoader.LoadAndCreateUI("ScoreDialog", (obj) =>
    {
      UIScoreDialog dialog = obj.GetComponent<UIScoreDialog>();
      dialog.Setup(scoreList, () =>
      {
        LoadScene(state);
      }, state.OnLoadSceneMainMenu);
    });
  }

  private IEnumerator SetupPlayer()
  {
    List<PlayerController> playerCtrlsTemp = new List<PlayerController>();

    for (int x = 0; x < pawnParent.childCount; x++)
    {
      PlayerController playerCtrl = pawnParent.GetChild(x).GetComponent<PlayerController>();
      playerCtrlsTemp.Add(playerCtrl);
    }

    //========================================================

    int rollCount = 0;
    bool complete = false;

    //order by red blue yellow green [Animation]
    while (rollCount < state.PlayerSlotIndexes.Count)
    {
      boardCtrl.SetBorderColor(state.PlayerSlotIndexes[rollCount].Index);
      complete = false;

      Action<int> onResult = (result) =>
      {
        Debug.Log("Dice result : " + result);

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

    Debug.Log("First player is " + playerTurnIndex);

    //========================================================

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

    //Clear old owned slot
    int previousIndex = playerCtrl.currentIndex - 1;
    BoardSlot boardSlot = boardCtrl.GetBoardSlot(previousIndex >= 0 ? previousIndex : boardCtrl.GetBoardSlotCount() - 1);
    boardSlot.ClearOwnedSlot(currentTurn);

    int nextIndex = playerCtrl.currentIndex + 1;

    //ถ้าเดินจนวนสนามแล้วจะรีเซ็ต index
    if (nextIndex >= boardCtrl.GetBoardSlotCount())
    {
      nextIndex = 0;
    }

    //ก่อนทีจะ move ต้องเช็ค player ในช่องก่อนหน้าก่อนว่ามีมั้ย ถ้ามีก็ให้ player ในช่องนั้น swap in slot
    boardSlot = boardCtrl.GetBoardSlot(nextIndex);
    boardSlot.MoveToSlot(currentTurn, (position) =>
    {
      playerCtrl.Move(nextIndex, position, () =>
      {
        diceResult--;

        if (diceResult == 0)
        {
          //Clear old owned slot
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
                  boardSlot.UpgradeSlot((int)playerCtrl.playerColor);
                  UpdateHealth(playerCtrl, -Const.COST_BUY_AREA);
                }, () =>
                {
                  onComplete?.Invoke();
                });
              }
              else if (playerCtrl.playerType == PlayerType.AI)
              {
                if (playerCtrl.GetHealth() > 1)
                {
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
              //buy area first time
              onBuyArea.Invoke();
            }
            else
            {
              if (boardSlot.CheckOwner((int)playerCtrl.playerColor))
              {
                if (boardSlot.CanUpgradeArea())
                {
                  //upgrade area
                  onBuyArea.Invoke();
                }
                else
                {
                  //auto turn end (max upgraded)
                  onComplete?.Invoke();
                }
              }
              else
              {
                //TakeDamage
                UpdateHealth(playerCtrl, -boardSlot.GetUpgradeCount());
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
