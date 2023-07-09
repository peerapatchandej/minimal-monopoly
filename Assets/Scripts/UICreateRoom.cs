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

  private List<SelectedPlayerData> playerSlotIndexes = new List<SelectedPlayerData>();
  private int playerCount = 0;

  public void Setup(ResourceLoader resourceLoader, Action onCreateMainMenu, Action<Monopoly.State> onLoadGameScene)
  {
    foreach (var slot in playerSlots)
    {
      slot.Setup((index) =>
      {
        playerCount++;
        UpdateStartButton();
        playerSlotIndexes.Add(new SelectedPlayerData { Index = index, Type = PlayerType.Player });
      }, (index) =>
      {
        playerCount++;
        UpdateStartButton();
        playerSlotIndexes.Add(new SelectedPlayerData { Index = index, Type = PlayerType.AI });
      }, (index) =>
      {
        playerCount--;
        UpdateStartButton();

        int foundedIndex = playerSlotIndexes.FindIndex(x => x.Index == index);
        if (foundedIndex != -1) playerSlotIndexes.RemoveAt(foundedIndex);
      });
    }

    startGame.interactable = false;
    startGame.onClick.AddListener(() =>
    {
      playerSlotIndexes.Sort((x, y) => x.Index.CompareTo(y.Index));

      onLoadGameScene?.Invoke(new Monopoly.State
      {
        ResourceLoader = resourceLoader,
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
