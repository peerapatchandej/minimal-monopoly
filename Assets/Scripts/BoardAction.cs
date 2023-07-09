using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardAction : MonoBehaviour
{
  [SerializeField]
  private DiceController DicePage = default;

  [SerializeField]
  private BuyAreaController BuyAreaPage = default;

  //[SerializeField]
  //private GameObject[] pages = default;

  public void EnableDicePage(Action<int> callback)
  {
    DicePage.Setup(callback);
    //DicePage.gameObject.SetActive(true);
  }

  public void EnableBuyArea(Action onBuyArea, Action onTurnEnd)
  {
    BuyAreaPage.Setup(onBuyArea, onTurnEnd);
  }

  //private void EnableDicePage(CenterAreaPage page)
  //{
  //  for (int i = 0; i < pages.Length; i++)
  //  {
  //    if (pages[i] != null) pages[i].SetActive((int)page == i);
  //  }
  //}
}
