using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceController : MonoBehaviour
{
  [SerializeField]
  private Dice dice = default;

  [SerializeField]
  private Button stop = default;

  public int result {  get; private set; }

  public void Setup(Action<int> callback)
  {
    if (dice)
    {
      dice.RollDice();
    }
    if (stop)
    {
      stop.interactable = true;
      stop.onClick.RemoveAllListeners();
      stop.onClick.AddListener(() =>
      {
        result = dice.StopRoll();
        callback?.Invoke(result);
      });
    }
  }
}
