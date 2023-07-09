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
    buyArea.gameObject.SetActive(false);
    turnEnd.gameObject.SetActive(false);
  }

  public void Setup(bool owned, bool canBuy, Action onBuyArea, Action onTurnEnd)
  {
    buyArea.gameObject.SetActive(canBuy);

    buyArea.onClick.RemoveAllListeners();
    buyArea.onClick.AddListener(() =>
    {
      buyArea.gameObject.SetActive(false);
      turnEnd.gameObject.SetActive(false);

      onBuyArea?.Invoke();
      onTurnEnd?.Invoke();
    });
  }
}
