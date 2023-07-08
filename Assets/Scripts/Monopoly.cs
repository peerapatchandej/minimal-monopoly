using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using Unity.VisualScripting;

public class Monopoly : MonoBehaviour
{
  public struct State
  {
    public List<int> PlayerSlotIndexes;
  }

  [SerializeField]
  private BoardController boardCtrl = default;

  [SerializeField]
  private CenterArea centerArea = default;

  [SerializeField]
  private GameObject[] pawnObjects = default;

  [SerializeField]
  private Transform pawnParent = default;

  [SerializeField]
  private List<PlayerController> playerCtrls = new List<PlayerController>();

  private static State state;

  private int maxPlayer = 4;
  private int playerTurn = -1;
  private int diceResult = 0;
  private bool moveNextComplete = false;

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
  }

  private IEnumerator Start()
  {
    StartCoroutine(SetupPlayer());

    yield return new WaitForSeconds(1f);

    //playerCtrls[0].Setup(Const.RED_PLAYER_INDEX);
    //playerCtrls[1].Setup(Const.BLUE_PLAYER_INDEX);

    //yield return new WaitForSeconds(1f);

    //playerTurn = 0;
    //diceResult = 1;

    //while (diceResult > 0)
    //{
    //  MoveNextSlot(() =>
    //  {
    //    moveNextComplete = true;
    //    diceResult--;
    //  });

    //  yield return new WaitUntil(() => moveNextComplete);

    //  moveNextComplete = false;
    //}

    //diceResult = 20;

    //while (diceResult > 0)
    //{
    //  MoveNextSlot(() =>
    //  {
    //    moveNextComplete = true;
    //    diceResult--;
    //  });

    //  yield return new WaitUntil(() => moveNextComplete);

    //  moveNextComplete = false;
    //}

    //diceResult = 20;

    //while (diceResult > 0)
    //{
    //  MoveNextSlot(() =>
    //  {
    //    moveNextComplete = true;
    //    diceResult--;
    //  });

    //  yield return new WaitUntil(() => moveNextComplete);

    //  moveNextComplete = false;
    //}

    //=======================================================================

    //playerTurn = 1;
    //diceResult = 16;

    //while (diceResult > 0)
    //{
    //  MoveNextSlot(() =>
    //  {
    //    moveNextComplete = true;
    //    diceResult--;
    //  });

    //  yield return new WaitUntil(() => moveNextComplete);

    //  moveNextComplete = false;
    //}
  }

  private IEnumerator SetupPlayer()
  {
    List<PlayerController> playerCtrlsTemp = new List<PlayerController>();

    foreach (var obj in pawnObjects)
    {
      GameObject player = Instantiate(obj, pawnParent);
      PlayerController playerCtrl = player.GetComponent<PlayerController>();
      playerCtrlsTemp.Add(playerCtrl);
    }

    //========================================================

    int rollCount = 0;
    bool complete = false;

    //order by red blue yellow green [Animation]
    while (rollCount < maxPlayer)
    {
      complete = false;

      RollDice((result) =>
      {
        Debug.Log("Dice result : " + result);

        if (diceResult < result)
        {
          diceResult = result;
          playerTurn = rollCount;
        }

        rollCount++;
        complete = true;
      });

      yield return new WaitUntil(() => complete);
    }

    Debug.Log("First player is " + playerTurn);

    //========================================================

    int sortCount = 0;
    int i = playerTurn;

    while (sortCount < maxPlayer)
    {
      if (i < maxPlayer)
      {
        playerCtrls.Add(playerCtrlsTemp[i]);
        i++;
        sortCount++;
      }
      else
      {
        i = 0;
      }
    }

    playerCtrlsTemp.Clear();
  }

  private void RollDice(Action<int> callback)
  {
    if (centerArea) centerArea.EnableDicePage(callback);
  }

  private void MoveNextSlot(Action onComplete = null)
  {
    //Clear old owned slot
    int previousIndex = playerCtrls[playerTurn].currentIndex - 1;
    BoardSlot boardSlot = boardCtrl.GetBoardSlot(previousIndex >= 0 ? previousIndex : boardCtrl.GetBoardSlotCount() - 1);
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
          //Debug.Log($"currentIndex : {playerCtrls[playerTurn].currentIndex}");

          //Action upgrade
          //check is corner then heal
          //else select upgrade or take damage

          if (!boardSlot.SlotHasUpgrade())
          {
            boardSlot.UpgradeSlot(playerTurn);
          }
          else
          {
            if (boardSlot.CheckOwner(playerTurn))
            {
              boardSlot.UpgradeSlot(playerTurn);
            }
            else
            {
              Debug.Log("Take Damage");
              boardSlot.ResetUpgradeSlot();
              //TakeDamage
            }
          }
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
