using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyAreaController : MonoBehaviour
{
  [SerializeField]
  private Button buyArea = default;

  [SerializeField]
  private Button turnEnd = default;

  private void Awake()
  {
    EnableButton(false);
  }

  public void Setup(Action onBuyArea, Action onTurnEnd)
  {
    EnableButton(true);

    buyArea.onClick.RemoveAllListeners();
    buyArea.onClick.AddListener(() =>
    {
      EnableButton(false);
      onBuyArea?.Invoke();
      onTurnEnd?.Invoke();
    });

    turnEnd.onClick.RemoveAllListeners();
    turnEnd.onClick.AddListener(() =>
    {
      EnableButton(false);
      onTurnEnd?.Invoke();
    });
  }

  private void EnableButton(bool value)
  {
    buyArea.gameObject.SetActive(value);
    turnEnd.gameObject.SetActive(value);
  }
}
