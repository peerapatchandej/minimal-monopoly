
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Dice : MonoBehaviour
{
  [SerializeField]
  private GameObject[] diceSides = default;

  private int result;
  private bool stopRoll = false;

  private void Awake()
  {
    for (int i = 0; i < diceSides.Length; i++)
    {
      diceSides[i].SetActive(false);
    }
  }

  public void RollDice()
  {
    stopRoll = false;
    StartCoroutine(StartRoll());
  }

  public int StopRoll()
  {
    stopRoll = true;
    return result + 1;
  }

  private IEnumerator StartRoll()
  {
    while (!stopRoll)
    {
      result = UnityEngine.Random.Range(0, diceSides.Length);
      SetActiveDice(result);

      yield return new WaitForSeconds(0.1f);
    }
  }

  private void SetActiveDice(int index)
  {
    for (int i = 0; i < diceSides.Length; i++)
    {
      diceSides[i].SetActive(i == index);
    }
  }
}
