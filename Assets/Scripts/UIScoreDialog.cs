using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScoreDialog : MonoBehaviour
{
  [SerializeField]
  private GameObject scoreListObj = default;

  [SerializeField]
  private Transform parent = default;

  [SerializeField]
  private Button rematch = default;

  [SerializeField]
  private Button mainMenu = default;

  public void Setup(List<PlayerColor> scoreList, Action onRematch, Action onMainMenu)
  {
    foreach (var score in scoreList)
    {
      GameObject obj = Instantiate(scoreListObj, parent);
    }

    rematch.onClick.AddListener(() =>
    {
      rematch.interactable = false;
      onRematch?.Invoke();
    });

    mainMenu.onClick.AddListener(() =>
    {
      mainMenu.interactable = false;
      onMainMenu?.Invoke();
    });
  }
}
