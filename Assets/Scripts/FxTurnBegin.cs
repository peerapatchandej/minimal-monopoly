using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;
using TMPro;

public class FxTurnBegin : MonoBehaviour
{
  [SerializeField]
  private Image image = default;

  [SerializeField]
  private TextMeshProUGUI turn = default;

  public void Setup(Color color, string text, Action onComplete = null)
  {
    turn.text = text;
    image.color = color;
    image.DOFade(1f, 0.4f);
    image.transform.DOScale(1f, 0.2f).OnComplete(() =>
    {
      onComplete?.Invoke();
      StartCoroutine(Delay());
    });
  }

  private IEnumerator Delay()
  {
    yield return new WaitForSeconds(1f);
    Destroy(gameObject);
  }
}