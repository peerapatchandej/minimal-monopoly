using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiceController : MonoBehaviour
{
  [SerializeField]
  private Transform dices = default;

  [SerializeField]
  private GameObject resultObj = default;

  [SerializeField]
  private TextMeshProUGUI resultText = default;

  [SerializeField]
  private Button stop = default;

  public int result { get; private set; }

  private void Awake()
  {
    resultObj.SetActive(false);
    stop.gameObject.SetActive(false);
  }

  public void Setup(Action<int> callback)
  {
    result = 0;
    resultObj.SetActive(false);

    for (int i = 0; i < dices.childCount - 2; i++)
    {
      Dice dice = dices.GetChild(i).GetComponent<Dice>();
      dice.RollDice();
    }

    stop.gameObject.SetActive(true);
    stop.onClick.RemoveAllListeners();
    stop.onClick.AddListener(() =>
    {
      for (int i = 0; i < dices.childCount - 2; i++)
      {
        Dice dice = dices.GetChild(i).GetComponent<Dice>();
        result += dice.StopRoll();
      }

      resultText.text = result.ToString();
      resultObj.SetActive(true);
      stop.gameObject.SetActive(false);

      callback?.Invoke(result);
    });
  }

  public IEnumerator SetupAI(Action<int> callback)
  {
    result = 0;
    resultObj.SetActive(false);
    stop.gameObject.SetActive(false);

    for (int i = 0; i < dices.childCount - 2; i++)
    {
      Dice dice = dices.GetChild(i).GetComponent<Dice>();
      dice.RollDice();
    }

    yield return new WaitForSeconds(1f);

    for (int i = 0; i < dices.childCount - 2; i++)
    {
      Dice dice = dices.GetChild(i).GetComponent<Dice>();
      result += dice.StopRoll();
    }

    resultText.text = result.ToString();
    resultObj.SetActive(true);

    callback?.Invoke(result);
  }
}