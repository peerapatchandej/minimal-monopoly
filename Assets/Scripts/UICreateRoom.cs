using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICreateRoom : MonoBehaviour
{
  [SerializeField]
  private UIPlayerSlot[] playerSlots = default;

  [SerializeField]
  private Button startGame = default;

  [SerializeField]
  private Button back = default;

  private List<int> playerSlotIndexes = new List<int>();

  public void Setup(Action OnCreateMainMenu, Action<List<int>> onLoadGameScene)
  {
    foreach (var slot in playerSlots)
    {
      slot.Setup((index) =>
      {
        playerSlotIndexes.Add(index);
      }, (index) =>
      {
        
      }, (index) =>
      {
        playerSlotIndexes.Remove(index);
      });
    }

    startGame.onClick.AddListener(() =>
    {
      playerSlotIndexes.Sort();
      onLoadGameScene?.Invoke(playerSlotIndexes);
      foreach (var slot in playerSlotIndexes)
      {
        Debug.Log(slot);
      }
    });

    back.onClick.AddListener(() =>
    {
      OnCreateMainMenu?.Invoke();
      back.interactable = false;
      Destroy(gameObject);
    });
  }
}
