using System;
using UnityEngine;

public class BoardAction : MonoBehaviour
{
  [SerializeField]
  private DiceController DicePage = default;

  [SerializeField]
  private BuyAreaController BuyAreaPage = default;

  public void EnableDicePage(Action<int> callback)
  {
    DicePage.Setup(callback);
  }

  public void EnableDicePageAI(Action<int> callback)
  {
    StartCoroutine(DicePage.SetupAI(callback));
  }

  public void EnableBuyArea(Action onBuyArea, Action onTurnEnd)
  {
    BuyAreaPage.Setup(onBuyArea, onTurnEnd);
  }
}
