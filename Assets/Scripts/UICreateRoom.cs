using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UICreateRoom : MonoBehaviour
{
  [SerializeField]
  private UIPlayerSlot[] playerSlots = default;

  [SerializeField]
  private SettingMenu edgeSetting = default;

  [SerializeField]
  private SettingMenu upgradeSlotSetting = default;

  [SerializeField]
  private SettingMenu healthSetting = default;

  [SerializeField]
  private DiceTypeSetting diceTypeSetting = default;

  [SerializeField]
  private SettingMenu diceSetting = default;

  [SerializeField]
  private Button startGame = default;

  [SerializeField]
  private Button back = default;

  private List<int> playerSlotIndexes = new List<int>();
  private int playerCount = 0;

  public void Setup(Action onCreateMainMenu, Action<Monopoly.State> onLoadGameScene)
  {
    foreach (var slot in playerSlots)
    {
      slot.Setup((index) =>
      {
        playerCount++;
        UpdateStartButton();
        playerSlotIndexes.Add(index);
      }, (index) =>
      {
        playerCount++;
        UpdateStartButton();
      }, (index) =>
      {
        playerCount--;
        UpdateStartButton();
        playerSlotIndexes.Remove(index);
      });
    }

    startGame.interactable = false;
    startGame.onClick.AddListener(() =>
    {
      playerSlotIndexes.Sort();
      onLoadGameScene?.Invoke(new Monopoly.State
      {
        PlayerSlotIndexes = playerSlotIndexes,
        PlayerHealth = healthSetting.GetValue(),
        MaxEdge = edgeSetting.GetValue(),
        MaxUpgradeSlot = upgradeSlotSetting.GetValue(),
        DiceType = diceTypeSetting.GetValue(),
        MaxDice = diceSetting.GetValue(),
        OnLoadSceneMainMenu = () =>
        {
          SceneManager.LoadScene("MainMenu");
        }
      });
    });

    back.onClick.AddListener(() =>
    {
      onCreateMainMenu?.Invoke();
      back.interactable = false;
      Destroy(gameObject);
    });
  }

  private void UpdateStartButton()
  {
    startGame.interactable = playerCount >= Const.MIN_PLAYER;
  }
}
